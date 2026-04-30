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
        self.embedding_processors = {}  # camera_id -> EmbeddingProcessor
        self.yolo_queues = {}        # camera_id -> Queue (frames para o YoloWorker)

        self.manager = Manager()
        self.log_queue = Queue(maxsize=2000)
        self.log_worker = LogWorker(self.log_queue)

        # Filas compartilhadas entre todas as câmeras
        self.face_queue = Queue(maxsize=100)   # crops de face -> FaceWorker
        self.result_queue = Queue(maxsize=200)  # embeddings -> EmbeddingProcessor
        self.zone_queue = Queue(maxsize=200)    # posições -> ZoneMonitor

        # FaceWorker: uma única instância, carrega InsightFace uma vez (250MB)
        self.face_worker = FaceWorker(self.face_queue, self.result_queue)
        self.face_worker.start()

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
            face_queue=self.face_queue,  # passa a face_queue para teste
        )
        cam_thread.start()
        self.camera_threads[camera_id] = cam_thread

        # YoloWorker: YOLO isolado por câmera (tracking correto)
        yolo_worker = YoloWorker(
            camera_id=camera_id,
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

        # EmbeddingProcessor: dedup e envio pro Rabbit por câmera
        emb_processor = EmbeddingProcessor(
            camera_id=camera_id,
            result_queue=self.result_queue,
        )
        emb_processor.start()
        self.embedding_processors[camera_id] = emb_processor

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
            self.zone_monitors,
            self.embedding_processors,
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

        if hasattr(self, "log_worker") and self.log_worker.is_alive():
            self.log_worker.stop()
            self.log_worker.join(timeout=5)

        if hasattr(self, "manager"):
            self.manager.shutdown()

        print("✅ Tudo encerrado.")

    def remove_camera(self, camera_id):
        self.stop_camera(camera_id)