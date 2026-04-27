from multiprocessing import Queue, Manager
from workers.camera_process import CameraProcess
from workers.inference_worker import InferenceWorker
from infrastructure.log_worker import LogWorker
from utils.has_feature import has_feature

NUM_INFERENCE_WORKERS = 2  # ajuste conforme sua VRAM

class MonitorManager:
    def __init__(self):
        self.workers = {}
        self.manager = Manager()
        self.log_queue = Queue(maxsize=2000)
        self.log_worker = LogWorker(self.log_queue)
        self.shared_person = self.manager.dict()

        # Filas centrais compartilhadas entre todas as câmeras
        self.inference_queue = Queue(maxsize=50)
        self.result_queue = Queue(maxsize=200)

        # Inicia os workers de inferência
        self.inference_workers = []
        for _ in range(NUM_INFERENCE_WORKERS):
            w = InferenceWorker(self.inference_queue, self.result_queue)
            w.start()
            self.inference_workers.append(w)

    def add_camera(self, camera):
        features = self.allowed_features(camera)

        process = CameraProcess(
            camera_id=camera["Id"],
            rtsp_url=camera["Connection"],
            sector_id=camera["SectorId"],
            roi=camera["Roi"],
            features=features,
            log_queue=self.log_queue,
            shared_person=self.shared_person,
            inference_queue=self.inference_queue,
            result_queue=self.result_queue,
        )

        process.start()
        self.workers[camera["Id"]] = process

    def allowed_features(self, cam):
        return {
            "environment_monitoring": has_feature(cam, 1),
            "incident_recording": has_feature(cam, 2),
            "dwell_time_monitoring": has_feature(cam, 3)
        }

    def stop_all(self):
        print("🛑 Parando câmeras...")
        processes = list(self.workers.values())
        for p in processes:
            p.stop()
        for p in processes:
            p.join(timeout=3)
            if p.is_alive():
                p.terminate()
                p.join()
        self.workers.clear()

        # Para os InferenceWorkers com sinal None
        print("🛑 Parando InferenceWorkers...")
        for _ in self.inference_workers:
            self.inference_queue.put(None)
        for w in self.inference_workers:
            w.join(timeout=5)
            if w.is_alive():
                w.terminate()

        if hasattr(self, 'log_worker') and self.log_worker.is_alive():
            self.log_worker.stop()
            self.log_worker.join(timeout=5)

        if hasattr(self, 'manager'):
            self.manager.shutdown()

        print("✅ Tudo encerrado.")

    def stop_camera(self, camera_id):
        process = self.workers.pop(camera_id, None)
        if process:
            process.stop()
            process.join(timeout=5)
            if process.is_alive():
                process.terminate()
                process.join()

    def removeCamera(self, camera_id):
        self.stop_camera(camera_id)