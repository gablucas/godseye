from asyncio import subprocess
import json
import os

from pydantic import json
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from threading import Thread, Event
import numpy as np
import time
import cv2

from infrastructure.ffmpeg_capture import ffmpeg_capture


class CameraThread(Thread):
    """
    Thread leve de captura: lê frames do ffmpeg e envia para a fila
    exclusiva do YoloWorker desta câmera.

    Usa Thread (não Process) porque é I/O-bound — leitura de pipe ffmpeg
    libera o GIL, então não há perda de paralelismo.
    """

    def __init__(self, camera_id, rtsp_url, features, yolo_queue, width, height):
        super().__init__(daemon=True)
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.features = features
        self.yolo_queue = yolo_queue
        self._stop_event = Event()
        self.width = width
        self.height = height

    def stop(self):
        self._stop_event.set()

    def run(self):

        if not (self.features.get("environment_monitoring") or
                self.features.get("dwell_time_monitoring")):
            return

        process = ffmpeg_capture(
            rtsp_url=self.rtsp_url,
            width=self.width,
            height=self.height,
            cameraId=self.camera_id,
            features=self.features,
            record_path=f"records/{self.camera_id}",
        )

        frame_size = self.width * self.height * 3
        TARGET_FPS = 10.0
        frame_interval = 1.0 / TARGET_FPS
        last_sent = 0.0
        frame_counter = 0;

        try:
            while not self._stop_event.is_set():
                frame_counter += 1

                raw = process.stdout.read(frame_size)
                if not raw or len(raw) < frame_size:
                    time.sleep(0.05)
                    continue

                if process.poll() is not None:
                    print(f"❌ ffmpeg morreu (câmera {self.camera_id})")
                    break

                now = time.time()
                if now - last_sent < frame_interval:
                    continue

                last_sent = now
                frame = np.frombuffer(raw, np.uint8).reshape((self.height, self.width, 3))

                # filename = f"debug_faces/teste_{int(time.time()*1000)}.jpg"
                # cv2.imwrite(filename, frame)
                # print('FACE RCONHECIDA E SALVA EM', filename)

                # print(f"📸 Câmera {self.camera_id} capturou frame {frame.shape} — {frame_counter}")

                if not self.yolo_queue.full():
                    self.yolo_queue.put_nowait(frame)
                else:
                    print(f"[⚠] yolo_queue cheia — câmera {self.camera_id} descartou frame")

        finally:
            process.kill()
