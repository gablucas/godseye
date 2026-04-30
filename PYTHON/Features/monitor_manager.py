from multiprocessing import Queue, Manager
from threading import Thread
from Features.camera_thread import CameraThread
from Features.yolo_worker import YoloWorker
from Features.face_worker import FaceWorker
from Features.zone_monitor import ZoneMonitor
from Features.embedding_processor import EmbeddingProcessor
from infrastructure.log_worker import LogWorker
from utils.has_feature import has_feature


class MonitorManager:
    def __init__(self):
        self.camera_threads = {}     # camera_id -> CameraThread
        self.yolo_workers = {}       # camera_id -> YoloWorker
        self.zone_monitors = {}      # camera_id -> ZoneMonitor
        self.yolo_queues = {}        # camera_id -> Queue (frames para o YoloWorker)

        self.manager = Manager()
        self.log_queue = Queue(maxsize=2000)
        self.log_worker = LogWorker(self.log_queue)

        # Filas compartilhadas entre todas as câmeras
        self.face_queue = Queue(maxsize=100)   # crops de face -> FaceWorker
        self.result_queue = Queue(maxsize=200)  # embeddings -> EmbeddingProcessor
        self.zone_queue = Queue(maxsize=200)    # posições -> ZoneMonitor

        # FaceWorker: única instância, carrega InsightFace uma vez (250MB)
        self.face_worker = FaceWorker(self.face_queue, self.result_queue)
        self.face_worker.start()

        # Embedding Processor: única instância
        self.embedding_processor = EmbeddingProcessor(result_queue=self.result_queue)
        self.embedding_processor.start()

    def add_camera(self, camera):
        camera_id = camera["Id"]
        features = self._allowed_features(camera)

        # Fila exclusiva de frames para o YoloWorker desta câmera
        yolo_queue = Queue(maxsize=30)
        self.yolo_queues[camera_id] = yolo_queue

        # Thread de captura: lê frames e envia para yolo_queue
        cam_thread = CameraThread(
            camera_id=camera_id,
            rtsp_url=camera["Connection"],
            features=features,
            yolo_queue=yolo_queue,
            width=640,
            height=480
        )
        cam_thread.start()
        self.camera_threads[camera_id] = cam_thread

        # YoloWorker: YOLO isolado por câmera (tracking correto)
        yolo_worker = YoloWorker(
            camera = camera,
            yolo_queue=yolo_queue,
            face_queue=self.face_queue,
            zone_queue=self.zone_queue,
        )
        yolo_worker.start()
        self.yolo_workers[camera_id] = yolo_worker

        # ZoneMonitor: detecta intrusão/dwell por câmera
        # zone_monitor = ZoneMonitor(
        #     camera_id=camera_id,
        #     roi=camera["Roi"],
        #     zone_queue=self.zone_queue,
        # )
        # zone_monitor.start()
        # self.zone_monitors[camera_id] = zone_monitor

    def _allowed_features(self, cam):
        return {
            "environment_monitoring": has_feature(cam, 1),
            "incident_recording": has_feature(cam, 2),
            "dwell_time_monitoring": has_feature(cam, 3),
        }

    def stop_camera(self, camera_id):
        for collection in [
            self.camera_threads,
            self.yolo_workers,
            self.zone_monitors
        ]:
            worker = collection.pop(camera_id, None)
            if worker:
                worker.stop()
                worker.join(timeout=5)
                if worker.is_alive():
                    worker.terminate()

    def stop_all(self):
        print("🛑 Parando câmeras...")
        for camera_id in list(self.camera_threads.keys()):
            self.stop_camera(camera_id)

        print("🛑 Parando FaceWorker...")
        self.face_queue.put(None)
        self.face_worker.join(timeout=5)
        if self.face_worker.is_alive():
            self.face_worker.terminate()

        print("🛑 Parando EmbeddingProcessor...")
        self.result_queue.put(None)  # sinaliza para sair do loop de batch
        self.embedding_processor.join(timeout=5)
        if self.embedding_processor.is_alive():
            self.embedding_processor.terminate()

        if hasattr(self, "log_worker") and self.log_worker.is_alive():
            self.log_worker.stop()
            self.log_worker.join(timeout=5)

        if hasattr(self, "manager"):
            self.manager.shutdown()

        print("✅ Tudo encerrado.")

    def remove_camera(self, camera_id):
        self.stop_camera(camera_id)

    def get_face_rois(self, cameras):
        face_rois = {}
        for cam in cameras:
            roi = cam.get("Roi") or []
            face_roi = next((r for r in roi if r["RoiType"] == 1), None)
            if face_roi:
                coords = face_roi["Coordinates"]
                face_rois[cam["Id"]] = {
                    "min_width": coords["Width"] * 640,
                    "min_height": coords["Height"] * 360,
                }

        return face_rois