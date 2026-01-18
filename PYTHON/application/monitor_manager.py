# stream/monitor_manager.py

from threading import Thread, Lock
from utils.has_feature import has_feature
from workers.camera_worker import CameraWorker

class MonitorManager:
    def __init__(self):
        self.workers = {}
        self.threads = {}

        self.active_users = {}
        self.active_users_lock = Lock()

    def start_monitoring(self, face_model, cameras, matcher, log_sender):
        for cam in cameras:

            print("########################################")
            print(f"Starting camera {cam['Id']}...")
            print("FEATURES:")


            environment_monitoring = has_feature(cam, 1)
            incident_recording = has_feature(cam, 2)

            print(f"Monitoramento Ambiental: {'SIM' if environment_monitoring else 'NÃO'}")
            print(f"Gravação de Incidentes: {'SIM' if incident_recording else 'NÃO'}")
            print("########################################")

            print(f"Camera {cam['Id']} - Incident Recording: {incident_recording}")

            worker = CameraWorker(
                face_model, 
                cam["Id"], 
                cam['Connection'], 
                cam['SectorId'], 
                matcher, 
                log_sender, 
                self.active_users, 
                self.active_users_lock,
                environment_monitoring=environment_monitoring,
                incident_recording=incident_recording
                )
            
            thread = Thread(target=worker.start)

            self.workers[cam["Id"]] = worker
            self.threads[cam["Id"]] = thread

            thread.start()

    def stop_camera(self, name):
        if name in self.workers:
            self.workers[name].stop()

    def stop_all(self):
        for w in self.workers.values():
            w.stop()
