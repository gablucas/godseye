import cv2
import time
from pathlib import Path
from threading import Thread
from datetime import datetime, timedelta

FACE_REPROCESS_TTL = 2.0      # segundos
VIDEO_POLL_INTERVAL = 1.0    # segundos
SKIP_N_FRAMES = 10          # pula frames para acelerar processamento

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


        video_start_time = self.parse_video_start_time(video_path)
        frame_index = 0

        while self.running:
            ret, frame = cap.read()
            if not ret:
                break

            if frame_index % SKIP_N_FRAMES == 0:
                timestamp_ms = cap.get(cv2.CAP_PROP_POS_MSEC)

                if timestamp_ms > 0:
                    video_time = timestamp_ms / 1000.0
                else:
                    video_time = frame_index / fps

                absolute_timestamp = video_start_time + timedelta(seconds=video_time)

                print(f"[FRAME] {video_path.name} frame={frame_index} time={video_time:.2f}s")
                self.process_frame(frame, video_time, absolute_timestamp)

            frame_index += 1

        cap.release()

    # =========================
    # PROCESSAMENTO DE FRAME
    # =========================
    def process_frame(self, frame, video_time, timestamp):
        faces = self.face_model.get_faces(frame)
        if not faces:
            return

        for face in faces:

            emb = face.normed_embedding
            self.process_embedding(emb, timestamp)

    # =========================
    # MATCH + LOG
    # =========================
    def process_embedding(self, emb, timestamp):
        user_id, score = self.matcher.match(emb)
        if user_id is None:
            return

        self.register_log(user_id, score, timestamp)

    # =========================
    # LOG CONSISTENTE
    # =========================
    def register_log(self, person_id, score, timestamp):
        with self.active_users_lock:
            data = self.active_users.get(person_id)

            if data and data["sectorId"] == self.sector_id:
                return

            self.active_users[person_id] = {
                "cameraId": self.camera_id,
                "sectorId": self.sector_id,
                "score": score,
                "createdAt": timestamp
            }

            print(f"[LOG] pessoa={person_id} cam={self.camera_id}")

            self.log_sender.send_log(
                cameraId=self.camera_id,
                personId=person_id,
                score=score,
                createdAt=timestamp.isoformat()
            )

    def parse_video_start_time(self, video_path: Path) -> datetime:
        # 15_20251217_085801.mkv
        name = video_path.stem
        _, date_part, time_part = name.split("_")

        return datetime.strptime(
            f"{date_part}_{time_part}",
            "%Y%m%d_%H%M%S"
        )