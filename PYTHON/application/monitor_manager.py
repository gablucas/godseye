# stream/monitor_manager.py

from threading import Lock, Thread
from utils.has_feature import has_feature
from workers.camera_worker import CameraWorker
from services.active_users_monitor import ActiveUsersMonitor


class MonitorManager:
    def __init__(self, face_model, face_matcher, cameras):
        self.face_model = face_model
        self.face_matcher = face_matcher
        self.cameras = cameras

        self.workers = {}

        # 🔥 ESTADO GLOBAL
        self.active_users = {}
        self.active_users_lock = Lock()

        # 🔥 MONITOR PERIÓDICO (1x só)
        # self.active_users_monitor = ActiveUsersMonitor(
        #     self.active_users,
        #     self.active_users_lock
        # )

    def start_monitoring_async(self):
        Thread(
            target=self.start_monitoring,
            daemon=True
        ).start()

    def start_monitoring(self):
        for cam in self.cameras:
            print(cam)

            print("##############################################################")
            print(f"INICIANDO MONITORAMENTO DA CAMERA {cam['Id']}...")

            environment_monitoring = has_feature(cam, 1)
            incident_recording = has_feature(cam, 2)
            dwell_time_monitoring = has_feature(cam, 3)
            print("FUNCIONALIDADES:")
            print(f"Env Monitoring: {'SIM' if environment_monitoring else 'NÃO'}")
            print(f"Incident Recording: {'SIM' if incident_recording else 'NÃO'}")
            print(f"Dwell Time: {'SIM' if dwell_time_monitoring else 'NÃO'}")
            print("##############################################################")

            worker = CameraWorker(
                face_model=self.face_model,
                cameraId=cam["Id"],
                rtsp_url=cam["Connection"],
                sectorId=cam["SectorId"],
                matcher=self.face_matcher,
                active_users=self.active_users,
                active_users_lock=self.active_users_lock,
                environment_monitoring=environment_monitoring,
                incident_recording=incident_recording,
                dwell_time_monitoring=dwell_time_monitoring
            )

            self.workers[cam["Id"]] = worker
            worker.start()

    def stop_camera(self, camera_id):
        if camera_id in self.workers:
            self.workers[camera_id].stop()

    def stop_all(self):
        for worker in self.workers.values():
            worker.stop()
