import time
from threading import Thread
from datetime import datetime
from zoneinfo import ZoneInfo
from infrastructure import log_queue
from infrastructure.logger import LogSender

class ActiveUsersMonitor:
    def __init__(self, active_users, active_users_lock):
        self.active_users = active_users
        self.active_users_lock = active_users_lock
        self.running = True

        Thread(target=self._loop, daemon=True).start()

    def _loop(self):
        while self.running:
            self.check_missing_users()
            time.sleep(60)

    def check_missing_users(self):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        # 🔒 lock curto e seguro
        with self.active_users_lock:
            snapshot = list(self.active_users.items())

        for personId, data in snapshot:
            diff = (now - data["last_seen"]).total_seconds() / 60

            if diff >= 10:
                log_queue.put((
                    LogSender.dotnet_send_missing_alert,
                    {
                        "personId": personId,
                        "cameraId": data["cameraId"],
                        "minutes": diff
                    }
                ))
