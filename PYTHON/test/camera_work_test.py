import time
import numpy as np
from threading import Thread, Lock
from datetime import datetime
from zoneinfo import ZoneInfo

from ultralytics import YOLO

from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender
from infrastructure.ffmpeg_capture import ffmpeg_capture


TRACK_TTL = 10
RECOGNITION_RETRY_INTERVAL = 1.5  # segundos


class CameraWorkerTest:
    def __init__(
        self,
        face_model: FaceModel,
        cameraId,
        rtsp_url,
        sectorId,
        matcher: FaceMatcher,
        log_sender: LogSender,
        active_users,
        active_users_lock,
        capture_fps: float = 30.0,
        width: int = 1280,
        height: int = 720
    ):
        self.face_model = face_model
        self.cameraId = cameraId
        self.url = rtsp_url
        self.sectorId = sectorId
        self.matcher = matcher
        self.log_sender = log_sender

        self.running = False

        self.active_users = active_users
        self.active_users_lock = active_users_lock

        self.capture_fps = capture_fps
        self.width = width
        self.height = height
        self.frame_size = width * height * 3

        self.model = YOLO("models/yolo11m.pt")

        # track_id -> state
        self.trackUsers = {}
        self.track_lock = Lock()

        self.last_sent_time = 0.0
        self.SEND_INTERVAL = 1.0 

    # ==========================
    # CONTROLE
    # ==========================
    def start(self):
        self.running = True
        print(f"[OK] Monitorando câmera {self.cameraId}")

        Thread(target=self.capture_loop, daemon=True).start()
        Thread(target=self.processing_loop, daemon=True).start()

        while self.running:
            time.sleep(1)

    def stop(self):
        self.running = False

    # ==========================
    # CAPTURE LOOP (tempo real)
    # ==========================
    def capture_loop(self):
        while self.running:
            process = ffmpeg_capture(
                rtsp_url=self.url,
                fps=self.capture_fps,
                width=self.width,
                height=self.height
            )

            try:
                while self.running:
                    now = time.time()

                    print("Frame recebido", time.time())

                    

                    raw = process.stdout.read(self.frame_size)
                    if len(raw) != self.frame_size:
                        print(f"[WARN] Frame incompleto ({self.cameraId})")
                        break

                    frame = np.frombuffer(raw, np.uint8).reshape(
                        (self.height, self.width, 3)
                    )

                    results = self.model.track(
                        frame,
                        persist=True,
                        conf=0.4,
                        iou=0.5,
                        device=0
                    )

                    if not results:
                        continue

                    r = results[0]
                    if r.boxes.id is None:
                        continue

                    boxes = r.boxes.xyxy.cpu().numpy()
                    ids = r.boxes.id.cpu().numpy()

                    with self.track_lock:
                        for box, track_id in zip(boxes, ids):
                            track_id = int(track_id)
                            x1, y1, x2, y2 = map(int, box)

                            crop = frame[y1:y2, x1:x2]
                            if crop.size == 0:
                                continue

                            user = self.trackUsers.get(track_id)

                            if user is None:
                                self.trackUsers[track_id] = {
                                    "last_crop": crop,
                                    "personId": None,
                                    "last_seen": now,
                                    "recognizing": False,
                                    "last_try": 0
                                }
                            else:
                                user["last_crop"] = crop
                                user["last_seen"] = now

                        # remove tracks expirados
                        self.trackUsers = {
                            k: v for k, v in self.trackUsers.items()
                            if now - v["last_seen"] < TRACK_TTL
                        }

            finally:
                process.kill()
                time.sleep(2)

    # ==========================
    # PROCESSING LOOP (assíncrono)
    # ==========================
    def processing_loop(self):
        while self.running:
            now = time.time()

            with self.track_lock:
                candidates = [
                    (track_id, user)
                    for track_id, user in self.trackUsers.items()
                    if user["personId"] is None
                    and not user["recognizing"]
                    and now - user["last_try"] > RECOGNITION_RETRY_INTERVAL
                ]

                for track_id, user in candidates:
                    user["recognizing"] = True
                    user["last_try"] = now

                    print(
                        "[PROCESS]",
                        track_id,
                        "recognizing",
                        user["recognizing"],
                        "last_try_delta",
                        now - user["last_try"]
                    )

            for track_id, user in candidates:
                self._process_track(track_id, user)

            time.sleep(0.01)

    # ==========================
    # FACE RECOGNITION
    # ==========================
    def _process_track(self, track_id, user):
        crop = user["last_crop"]

        h, w, _ = crop.shape
        if w < 80 or h < 80:
            self._unlock(track_id)
            return

        faces = self.face_model.get_faces(crop)
        if not faces:
            self._unlock(track_id)
            return

        face = faces[0]
        emb = face.normed_embedding
        user_id, score = self.matcher.match(emb)

        with self.track_lock:
            track = self.trackUsers.get(track_id)
            if track is None:
                return

            if user_id is not None:
                track["personId"] = user_id
                self.register_log(
                    user_id,
                    self.cameraId,
                    self.sectorId,
                    score
                )

            track["recognizing"] = False

            print("[MATCH]", user_id, score)

    def _unlock(self, track_id):
        with self.track_lock:
            user = self.trackUsers.get(track_id)
            if user:
                user["recognizing"] = False

    # ==========================
    # LOG
    # ==========================
    def register_log(self, personId, cameraId, sectorId, score):
        with self.active_users_lock:
            personData = self.active_users.get(personId)

            if personData is None or personData["sectorId"] != sectorId:
                self.active_users[personId] = {
                    "cameraId": cameraId,
                    "sectorId": sectorId,
                    "score": score
                }

                self.log_sender.dotnet_create_environment_monitoring_log(
                    cameraId=cameraId,
                    personId=personId,
                    score=score,
                    createdAt=datetime.now(
                        ZoneInfo("America/Sao_Paulo")
                    ).isoformat()
                )
