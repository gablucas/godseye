import cv2
import time
import os
from threading import Thread
from queue import Queue, Empty, Full

from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender


# =========================
# CONFIGURAÇÕES
# =========================
DEBUG_VIEW = False

PROCESS_EVERY_SECONDS = 0.8       # intervalo realista
RECONNECT_FAIL_LIMIT = 30

FRAME_QUEUE_SIZE = 1              # sempre o frame mais recente
FACE_QUEUE_SIZE = 64

FACE_WORKER_COUNT = 4             # threads de match/log

MAX_FACES_PER_FRAME = 3           # evita explosão de custo
FACE_REPROCESS_TTL = 2.0          # evita reprocessar a mesma pessoa


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
        self.face_queue = Queue(maxsize=FACE_QUEUE_SIZE)

        # cache simples de faces recentes (bbox → timestamp)
        self.recent_faces = {}

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

        for _ in range(FACE_WORKER_COUNT):
            Thread(target=self.face_worker, daemon=True).start()

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

            # mantém só o frame mais recente
            if self.frame_queue.full():
                try:
                    self.frame_queue.get_nowait()
                except Empty:
                    pass

            self.frame_queue.put(frame)

    # =========================
    # PROCESSAMENTO
    # =========================
    def processing_loop(self):
        last_process_time = 0

        while self.running:
            try:
                frame = self.frame_queue.get(timeout=1)
            except Empty:
                continue

            # drena frames antigos
            while not self.frame_queue.empty():
                try:
                    frame = self.frame_queue.get_nowait()
                except Empty:
                    break

            now = time.time()
            if now - last_process_time < PROCESS_EVERY_SECONDS:
                continue

            last_process_time = now

            print(f"INICIANDO A DETECÇÃO DE FACES: {now:.3f}")

            faces = self.face_model.get_faces(frame)
            if not faces:
                continue

            # prioriza faces maiores (mais próximas)
            faces.sort(
                key=lambda f: (f.bbox[2] - f.bbox[0]) * (f.bbox[3] - f.bbox[1]),
                reverse=True
            )

            faces = faces[:MAX_FACES_PER_FRAME]

            print(f"Faces processadas: {len(faces)}")

            for face in faces:
                face_key = self._face_hash(face)

                last_seen = self.recent_faces.get(face_key)
                if last_seen and now - last_seen < FACE_REPROCESS_TTL:
                    continue

                self.recent_faces[face_key] = now

                emb = face.normed_embedding

                try:
                    self.face_queue.put_nowait({
                        "emb": emb,
                        "ts": time.time()
                    })
                except Full:
                    print("[DROP] face_queue cheia — embedding descartado")

    # =========================
    # WORKERS DE MATCH
    # =========================
    def face_worker(self):
        print("[WORKER] iniciado")

        while self.running:
            try:
                item = self.face_queue.get(timeout=1)
            except Empty:
                continue

            emb = item["emb"]
            detect_ts = item["ts"]

            try:
                user_id, score = self.matcher.match(emb)
                now = time.time()
                latency = now - detect_ts

                if user_id is None:
                    print(f"[MATCH FAIL] latency={latency:.3f}s")
                    continue

                print(f"[MATCH OK] latency={latency:.3f}s")

                self.register_log(
                    user_id,
                    self.cameraId,
                    self.sectorId,
                    score
                )

            except Exception as e:
                print(f"[WORKER ERROR] {e}")
            finally:
                self.face_queue.task_done()

    # =========================
    # LOG
    # =========================
    def register_log(self, personId, cameraId, sectorId, score):
        with self.active_users_lock:
            personData = self.active_users.get(personId)

            if personData is None:
                print("[LOG] novo usuario")
            elif personData["sectorId"] != sectorId:
                print("[LOG] mudou de setor")
            else:
                print("[SKIP] usuario já ativo no setor")

            if personData is None or personData["sectorId"] != sectorId:
                self.active_users[personId] = {
                    "cameraId": cameraId,
                    "sectorId": sectorId,
                    "score": score
                }

                self.log_sender.send_log(
                    cameraId=cameraId,
                    personId=personId,
                    score=score
                )

    # =========================
    # UTIL
    # =========================
    def _face_hash(self, face):
        # hash simples baseado na bbox (bom o suficiente)
        x1, y1, x2, y2 = map(int, face.bbox)
        return f"{x1//10}-{y1//10}-{x2//10}-{y2//10}"
