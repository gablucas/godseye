# infrastructure/logger.py

import requests
from datetime import datetime
from zoneinfo import ZoneInfo
from config.settings import VERIFY_SSL

class LogSender:
    def __init__(self, api_url):
        self.api_url = api_url

    def send_log(self, cameraId, personId, score, createdAt):
        payload = {
            "cameraId": cameraId,
            "personId": personId,
            "score": float(score),
            "createdAt": createdAt
        }

        try:
            requests.post(
                f"{self.api_url}",
                json=payload,
                timeout=5,
                verify=VERIFY_SSL
            )
        except Exception as e:
            print(f"Falha ao enviar log: {e}")
