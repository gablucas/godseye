import cv2
import time
from pathlib import Path
from threading import Thread

FACE_REPROCESS_TTL = 2.0      # segundos
VIDEO_POLL_INTERVAL = 1.0    # segundos

class CameraWorker:
    def __init__(
        self,
        face_model,
        camera_id: int,
        sector_id: int,
        matcher,
        log_sender,
        active_users: dict,
        active_users_lock,
        records_root="records"
    ):
        self.face_model = face_model
        self.camera_id = camera_id
        self.sector_id = sector_id
        self.matcher = matcher
        self.log_sender = log_sender

        self.active_users = active_users
        self.active_users_lock = active_users_lock

        self.records_dir = Path(records_root) / str(camera_id)
        self.records_dir.mkdir(parents=True, exist_ok=True)

        self.running = False

        self.processed_videos = set()
        self.recent_faces = {}

    # =========================
    # CONTROLE
    # =========================
    def start(self):
        self.running = True
        Thread(target=self.video_scan_loop, daemon=True).start()
        print(f"[OK] CameraWorker OFFLINE ativo | câmera={self.camera_id}")

    def stop(self):
        self.running = False

    # =========================
    # LOOP PRINCIPAL
    # =========================
    def video_scan_loop(self):
        while self.running:
            videos = self._list_videos()

            for video in videos:
                if not self.running:
                    break

                if video.name in self.processed_videos:
                    continue

                if not self.is_video_stable(video):
                    print(f"[SKIP] {video.name} ainda sendo escrito")
                    continue

                print(f"[VIDEO] {video.name}")
                self.process_video(video)
                self.processed_videos.add(video.name)

            time.sleep(VIDEO_POLL_INTERVAL)

    # =========================
    # VERIFICA SE O VIDEO ESTÁ PRONTO PARA ANALISE
    # =========================

    def is_video_stable(self, path: Path, interval=2.0):
        try:
            size1 = path.stat().st_size
            time.sleep(interval)
            size2 = path.stat().st_size
            return size1 == size2
        except FileNotFoundError:
            return False

    # =========================
    # LISTAGEM ORDENADA
    # =========================
    def _list_videos(self):
        return sorted(
            self.records_dir.glob("*.mkv"),
            key=lambda f: f.stat().st_mtime
        )

    # =========================
    # PROCESSAMENTO COMPLETO
    # =========================
    def process_video(self, video_path: Path):
        cap = cv2.VideoCapture(str(video_path))
        if not cap.isOpened():
            print(f"[ERRO] Falha ao abrir {video_path.name}")
            return

        fps = cap.get(cv2.CAP_PROP_FPS)
        if fps <= 0:
            fps = 30.0

        frame_index = 0

        while self.running:
            ret, frame = cap.read()
            if not ret:
                break

            print(f"[FRAME] {video_path.name} frame={frame_index}")

            frame_index += 1
            video_time = frame_index / fps  # tempo REAL do vídeo

            self.process_frame(frame, video_time)

        cap.release()

    # =========================
    # PROCESSAMENTO DE FRAME
    # =========================
    def process_frame(self, frame, video_time):
        faces = self.face_model.get_faces(frame)
        if not faces:
            return

        for face in faces:
            face_key = self._face_hash(face)

            last_seen = self.recent_faces.get(face_key)
            if last_seen is not None and video_time - last_seen < FACE_REPROCESS_TTL:
                continue

            self.recent_faces[face_key] = video_time

            emb = face.normed_embedding
            self.process_embedding(emb)

    # =========================
    # MATCH + LOG
    # =========================
    def process_embedding(self, emb):
        user_id, score = self.matcher.match(emb)
        if user_id is None:
            return

        self.register_log(user_id, score)

    # =========================
    # LOG CONSISTENTE
    # =========================
    def register_log(self, person_id, score):
        with self.active_users_lock:
            data = self.active_users.get(person_id)

            if data and data["sectorId"] == self.sector_id:
                return

            self.active_users[person_id] = {
                "cameraId": self.camera_id,
                "sectorId": self.sector_id,
                "score": score
            }

            print(f"[LOG] pessoa={person_id} cam={self.camera_id}")

            self.log_sender.dotnet_create_environment_monitoring_log(
                cameraId=self.camera_id,
                personId=person_id,
                score=score
            )

    # =========================
    # HASH SIMPLES
    # =========================
    def _face_hash(self, face):
        x1, y1, x2, y2 = map(int, face.bbox)
        return f"{x1//10}-{y1//10}-{x2//10}-{y2//10}"
