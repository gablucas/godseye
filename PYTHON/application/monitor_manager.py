from httpcore import request
from utils.has_feature import has_feature
from workers.camera_process import CameraProcess
from multiprocessing import Queue
from infrastructure.log_worker import LogWorker

class MonitorManager:
    def __init__(self, cameras, face_matcher):
        self.cameras = cameras
        self.face_matcher = face_matcher
        self.workers = {}
        self.log_queue = Queue(maxsize=2000)
        self.log_worker = LogWorker(self.log_queue)

    def start_monitoring(self):

        self.log_worker.start()

        for cam in self.cameras:
            print("##############################################################")
            print(f"INICIANDO MONITORAMENTO DA CAMERA {cam['Id']}")

            features = self.allowed_features(cam)

            print("##############################################################")

            process = CameraProcess(
                camera_id=cam["Id"],
                rtsp_url=cam["Connection"],
                sector_id=cam["SectorId"],
                features=features,
                face_matcher=self.face_matcher,
                log_queue=self.log_queue
            )

            process.start()
            self.workers[cam["Id"]] = process


    def allowed_features(self, cam):
        return {
            "environment_monitoring": has_feature(cam, 1),
            "incident_recording": has_feature(cam, 2),
            "dwell_time_monitoring": has_feature(cam, 3)
        }

    def stop_camera(self, camera_id):
        if camera_id in self.workers:
            self.workers[camera_id].stop()
            self.workers[camera_id].join()

    def stop_all(self):
        for process in self.workers.values():
            process.stop()
            process.join()

        print("🛑 Parando LogWorker")
        self.log_worker.stop()
        self.log_worker.join()
