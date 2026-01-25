import cv2
import time
import os
from threading import Thread
from queue import Queue, Empty
from typing import Tuple

from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender


# =========================
# CONFIGURAÇÕES
# =========================
DEBUG_VIEW = False

PROCESS_EVERY_SECONDS = 0.7
RECONNECT_FAIL_LIMIT = 30
FRAME_QUEUE_SIZE = 1

TRACK_TTL_SECONDS = 2.0       # quanto tempo um track pode ficar sem aparecer
IOU_THRESHOLD = 0.3           # overlap mínimo para considerar o mesmo rosto


class CameraWorker:
    def __init__(
        self,
        face_model: FaceModel,
        cameraId: int,
        rtsp_url: str,
        sectorId: int,
        matcher: FaceMatcher,
        log_sender: LogSender,
        active_users: dict,
        active_users_lock
    ):
        self.face_model = face_model
        self.cameraId = cameraId
        self.url = rtsp_url
        self.sectorId = sectorId
        self.matcher = matcher
        self.log_sender = log_sender

        self.active_users = active_users
        self.active_users_lock = active_users_lock

        self.running = False
        self.frame_queue = Queue(maxsize=FRAME_QUEUE_SIZE)

        # TRACKING
        self.tracks = {}           # track_id -> track data
        self.next_track_id = 1

    # =========================
    # CONTROLE
    # =========================
    def start(self):
        self.running = True
        cap = self._open_capture()

        if not cap or not cap.isOpened():
            print(f"[ERRO] Não abriu câmera {self.cameraId}")
            return

        print(f"[OK] Monitorando câmera: {self.cameraId}")

        Thread(target=self.capture_loop, args=(cap,), daemon=True).start()
        Thread(target=self.processing_loop, daemon=True).start()

        try:
            while self.running:
                time.sleep(1)
        finally:
            cap.release()

    def stop(self):
        self.running = False

    # =========================
    # CAPTURA
    # =========================
    def _open_capture(self):
        if DEBUG_VIEW:
            return cv2.VideoCapture(0, cv2.CAP_DSHOW)

        os.environ["OPENCV_FFMPEG_CAPTURE_OPTIONS"] = (
            "rtsp_transport;tcp|"
            "fflags;nobuffer|"
            "flags;low_delay|"
            "analyzeduration;1000000|"
            "probesize;1000000"
        )

        cap = cv2.VideoCapture(self.url, cv2.CAP_FFMPEG)
        cap.set(cv2.CAP_PROP_BUFFERSIZE, 2)
        return cap

    def capture_loop(self, cap):
        fail_count = 0

        while self.running:
            ret, frame = cap.read()

            if not ret or frame is None:
                fail_count += 1
                if fail_count >= RECONNECT_FAIL_LIMIT:
                    print(f"[WARN] Reconectando câmera {self.cameraId}")
                    cap.release()
                    time.sleep(2)
                    cap.open(self.url)
                    fail_count = 0
                continue

            fail_count = 0

            if self.frame_queue.full():
                try:
                    self.frame_queue.get_nowait()
                except Empty:
                    pass

            self.frame_queue.put(frame)

    # =========================
    # PROCESSAMENTO + TRACKING
    # =========================
    def processing_loop(self):
        last_process_time = 0

        while self.running:
            try:
                frame = self.frame_queue.get(timeout=1)
            except Empty:
                continue

            now = time.time()
            if now - last_process_time < PROCESS_EVERY_SECONDS:
                continue

            last_process_time = now

            faces = self.face_model.get_faces(frame)
            if not faces:
                self._cleanup_tracks(now)
                continue

            detections = []
            for face in faces:
                x1, y1, x2, y2 = face.bbox
                detections.append((x1, y1, x2, y2, face))

            self._update_tracks(detections, now)

            # evita CPU 100%
            time.sleep(0.005)

    # =========================
    # TRACKING
    # =========================
    def _update_tracks(self, detections, now: float):
        used_tracks = set()

        for x1, y1, x2, y2, face in detections:
            best_iou = 0
            best_track_id = None

            for track_id, track in self.tracks.items():
                iou = self._iou((x1, y1, x2, y2), track["bbox"])
                if iou > best_iou:
                    best_iou = iou
                    best_track_id = track_id

            if best_iou >= IOU_THRESHOLD and best_track_id is not None:
                track = self.tracks[best_track_id]
                track["bbox"] = (x1, y1, x2, y2)
                track["last_seen"] = now
                used_tracks.add(best_track_id)

                if not track["embedding_done"]:
                    self._process_face(face, track)
            else:
                track_id = self.next_track_id
                self.next_track_id += 1

                self.tracks[track_id] = {
                    "bbox": (x1, y1, x2, y2),
                    "last_seen": now,
                    "embedding_done": False
                }

                # self._process_face(face, self.tracks[track_id])
                used_tracks.add(track_id)

        self._cleanup_tracks(now)

    def _cleanup_tracks(self, now: float):
        expired = [
            tid for tid, track in self.tracks.items()
            if now - track["last_seen"] > TRACK_TTL_SECONDS
        ]
        for tid in expired:
            del self.tracks[tid]

    # =========================
    # FACE PROCESSING
    # =========================
    def _process_face(self, face, track):
        emb = face.normed_embedding
        user_id, score = self.matcher.match(emb)

        track["embedding_done"] = True

        if user_id is None:
            return

        self.register_log(user_id, self.cameraId, self.sectorId, score)

    # =========================
    # IOU
    # =========================
    def _iou(self, a: Tuple[int, int, int, int], b: Tuple[int, int, int, int]) -> float:
        ax1, ay1, ax2, ay2 = a
        bx1, by1, bx2, by2 = b

        inter_x1 = max(ax1, bx1)
        inter_y1 = max(ay1, by1)
        inter_x2 = min(ax2, bx2)
        inter_y2 = min(ay2, by2)

        inter_area = max(0, inter_x2 - inter_x1) * max(0, inter_y2 - inter_y1)
        area_a = (ax2 - ax1) * (ay2 - ay1)
        area_b = (bx2 - bx1) * (by2 - by1)

        union = area_a + area_b - inter_area
        return inter_area / union if union > 0 else 0.0

    # =========================
    # LOG
    # =========================
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
                    score=score
                )
