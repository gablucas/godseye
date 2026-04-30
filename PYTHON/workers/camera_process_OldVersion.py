# stream/camera_worker.py
from multiprocessing import Process, Queue, Event
from threading import Thread, Lock

import time
from datetime import datetime
from zoneinfo import ZoneInfo
import cv2
from ultralytics import YOLO
import supervision as sv

from infrastructure.send_extracted_embedding import SendExtractedEmbedding
from schemas.extracted_embedding import ExtractedEmbedding
from services.face_recognition_service import FaceModel
import numpy as np
from infrastructure.ffmpeg_capture import ffmpeg_capture

TIMEOUT = 5
DEBUG_VIEW = False
PROCESS_EVERY_N_FRAMES = 1

# Similaridade mínima para considerar dois embeddings como a mesma pessoa
EMBEDDING_SIMILARITY_THRESHOLD = 0.6


def cosine_similarity(a: np.ndarray, b: np.ndarray) -> float:
    """Retorna a similaridade cosseno entre dois embeddings normalizados."""
    return float(np.dot(a, b))


class CameraProcess(Process):
    def __init__(
        self,
        camera_id,
        rtsp_url,
        sector_id,
        roi,
        features=None,
        log_queue=None,
    ):
        super().__init__()
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.sector_id = sector_id
        self.roi = roi
        self.features = features or {}
        self.stop_event = Event()
        self.log_queue = log_queue
        self.embedding_sender = None
        self.frame_queue = Queue(maxsize=1)
        self.width = 640
        self.height = 360

    def stop(self):
        self.stop_event.set()

    def run(self):
        self.frame_lock = Lock()
        self.init_models()

        self.embedding_sender = SendExtractedEmbedding()
        capture_thread = Thread(target=self.capture_loop)
        inference_thread = Thread(target=self.inference_loop)

        capture_thread.start()
        inference_thread.start()

        while not self.stop_event.is_set():
            time.sleep(1)

    def init_models(self):
        self.yolo = YOLO("yolo26s.pt")
        self.yolo.to("cuda")
        self.face_model = FaceModel()

        self.box_annotator = sv.BoxAnnotator()
        self.label_annotator = sv.LabelAnnotator()

        # Mapeia track_id -> embedding (normed) extraído na primeira detecção
        self.embedding_by_track_id: dict[int, np.ndarray] = {}

        # Controla quais tracks estão ativos (track_id -> timestamp da última vez visto)
        self.active_tracks: dict[int, float] = {}

        self.last_run = 0.0
        self.last_detection = 0.0
        self.target_fps = 1.0

        self.last_detection = 0
        self.bg = cv2.createBackgroundSubtractorMOG2(
            history=500,
            varThreshold=16,
            detectShadows=False
        )

    def capture_loop(self):
        process = ffmpeg_capture(
            rtsp_url=self.rtsp_url,
            width=self.width,
            height=self.height,
            cameraId=self.camera_id,
            features=self.features,
            record_path=f"records/{self.camera_id}"
        )

        if (
            self.features.get("environment_monitoring", False)
            or self.features.get("dwell_time_monitoring", False)
        ):
            frame_size = self.width * self.height * 3

            try:
                while not self.stop_event.is_set():
                    raw = process.stdout.read(frame_size)
                    if not raw or len(raw) < frame_size:
                        time.sleep(0.1)
                        continue

                    if process.poll() is not None:
                        print("❌ ffmpeg morreu")
                        break

                    frame = np.frombuffer(raw, np.uint8).reshape(
                        (self.height, self.width, 3)
                    )

                    self.frame_queue.put(frame)

            finally:
                process.kill()

    def movement_detection(self, frame, now):
        FPS_IDLE = 1.0
        FPS_ACTIVE = 10.0
        FPS_STEADY = 5.0
        COOLDOWN_TIME = 2.0

        mask = self.bg.apply(frame)
        motion_pixels = cv2.countNonZero(mask)

        IS_HIGH_MOTION = motion_pixels > 5000
        IS_LOW_MOTION = motion_pixels > 500

        has_people = len(self.active_tracks) > 0

        if IS_HIGH_MOTION:
            self.last_detection = now
            self.target_fps = FPS_ACTIVE
            print(f"🚀 MODO ATIVO (Alta movimentação: {motion_pixels})")

        elif has_people:
            self.target_fps = FPS_STEADY
            self.last_detection = now
            print("👀 MODO STEADY (Pessoas paradas)")

        elif IS_LOW_MOTION or (now - self.last_detection < COOLDOWN_TIME):
            self.target_fps = FPS_STEADY

        else:
            self.target_fps = FPS_IDLE
            print("💤 MODO IDLE")

        return motion_pixels > 500

    def inference_loop(self):
        while not self.stop_event.is_set():
            now = time.time()

            frame = None
            while not self.frame_queue.empty():
                try:
                    frame = self.frame_queue.get_nowait()
                except Exception:
                    break

            if frame is None:
                continue

            self.movement_detection(frame, now)

            print(
                f"Target FPS: {self.target_fps:.2f}, "
                f"Active Tracks: {len(self.active_tracks)}, "
                f"Queue Size: {self.frame_queue.qsize()}"
            )

            frame_interval = 1.0 / self.target_fps
            elapsed = now - self.last_run

            if elapsed < frame_interval:
                time.sleep(frame_interval - elapsed)

            self.last_run = time.time()
            print("PROCESSANDO FRAME")

            self.process_frame(frame)

    def _find_existing_track_by_embedding(
        self, new_emb: np.ndarray
    ) -> int | None:
        """
        Verifica se o embedding recebido já existe em algum track ativo.
        Retorna o track_id com maior similaridade acima do threshold, ou None.
        """

        print("🔍 Verificando se embedding já existe em algum track ativo...")
        best_track_id = None
        best_score = EMBEDDING_SIMILARITY_THRESHOLD

        for track_id, stored_emb in self.embedding_by_track_id.items():
            score = cosine_similarity(new_emb, stored_emb)
            if score > best_score:
                best_score = score
                best_track_id = track_id

        return best_track_id

    def process_frame(self, frame):
        now = time.time()

        results = self.yolo.track(
            source=frame,
            persist=True,
            tracker="botsort.yaml",
            classes=[0],
            verbose=False
        )

        detections = sv.Detections.from_ultralytics(results[0])

        if detections.tracker_id is None:
            tracker_ids_on_frame = []
            detections.tracker_id = np.array([])
        else:
            tracker_ids_on_frame = detections.tracker_id.tolist()

        # ==================================================
        # 1. PROCESSAR TRACKS ATIVOS
        # ==================================================
        if len(tracker_ids_on_frame) > 0:
            for i, track_id in enumerate(tracker_ids_on_frame):
                x1, y1, x2, y2 = detections.xyxy[i].astype(int)

                # Atualiza timestamp do track
                self.active_tracks[track_id] = now

                # CASO A: TRACK JÁ TEM EMBEDDING ASSOCIADO — nada a fazer
                if track_id in self.embedding_by_track_id:
                    continue

                # CASO B: NOVO TRACK — tentar extrair embedding facial
                crop = frame[y1:y2, x1:x2]
                if crop.size == 0:
                    continue

                faces = self.face_model.get_faces(crop)
                if not faces:
                    continue

                # Verificar ROI de rosto, se configurado
                face_roi = None
                if self.roi:
                    face_roi = next(
                        (r for r in self.roi if r["RoiType"] == 1), None
                    )

                if face_roi:
                    coords = face_roi["Coordinates"]
                    frame_h, frame_w = frame.shape[:2]

                    min_face_w_px = coords["Width"] * frame_w
                    min_face_h_px = coords["Height"] * frame_h

                    def face_area(f):
                        x1f, y1f, x2f, y2f = f.bbox
                        return (x2f - x1f) * (y2f - y1f)

                    face = max(faces, key=face_area)
                    x1f, y1f, x2f, y2f = face.bbox.astype(int)
                    face_width = x2f - x1f
                    face_height = y2f - y1f

                    if face_width < min_face_w_px or face_height < min_face_h_px:
                        continue

                    emb = face.normed_embedding
                else:
                    emb = faces[0].normed_embedding

                # Checar se esse embedding já aparece em outro track ativo
                # (pode acontecer se o YOLO trocou o track_id da mesma pessoa)
                duplicate_track = self._find_existing_track_by_embedding(emb)
                

                if duplicate_track is not None:
                    print(
                        f"🔄 Track {track_id} parece ser o mesmo que {duplicate_track} "
                        f"(embedding similar). Reaproveitando embedding."
                    )
                    self.embedding_by_track_id[track_id] = (
                        self.embedding_by_track_id[duplicate_track]
                    )
                    continue

                # Novo embedding — associar ao track e enviar
                self.embedding_by_track_id[track_id] = emb


                print(f"✅ Novo embedding para Track {track_id} (Camera {self.camera_id})")
                payload = ExtractedEmbedding(
                    CameraId=self.camera_id,
                    Embedding=emb.tolist(),
                    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
                )

                self.embedding_sender.send_extracted_embedding(payload)
                print(f"✅ Embedding enviado para track {track_id}")

        # ==================================================
        # 2. LIMPEZA DE TRACKS PERDIDOS
        # ==================================================
        current_tracks_set = set(tracker_ids_on_frame)
        known_tracks = list(self.active_tracks.keys())

        for tid in known_tracks:
            if tid not in current_tracks_set:
                print(f"🧹 Limpando Track {tid} (Saiu de cena)")
                self.active_tracks.pop(tid, None)
                self.embedding_by_track_id.pop(tid, None)