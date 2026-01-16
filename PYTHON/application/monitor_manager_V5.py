# stream/monitor_manager.py

from threading import Thread, Lock
from workers.camera_worker import CameraWorker

class MonitorManager:
    def __init__(self):
        self.workers = {}
        self.threads = {}

        self.active_users = {}
        self.active_users_lock = Lock()

    def start_monitoring(self, face_model, cameras, matcher, log_sender):
        """
        cameras = [
          { 'name': 'cam1', 'rtsp': 'rtsp://...' },
          ...
        ]
        """

        for cam in cameras:
            worker = CameraWorker(face_model, cam["Id"], cam['SectorId'], matcher, log_sender, self.active_users, self.active_users_lock)
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
