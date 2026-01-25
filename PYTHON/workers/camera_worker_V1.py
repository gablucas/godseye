# stream/camera_worker.py

import cv2
import time
import os
from threading import Thread
from queue import Queue
from datetime import datetime
from zoneinfo import ZoneInfo

from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender

TIMEOUT = 5
DEBUG_VIEW = False
PROCESS_EVERY_N_FRAMES = 3


class CameraWorker:
    def __init__(self, face_model, cameraId, rtsp_url, sectorId, matcher: FaceMatcher, log_sender: LogSender, active_users, active_users_lock):
        self.face_model = face_model
        self.cameraId = cameraId
        self.url = rtsp_url
        self.sectorId = sectorId
        self.matcher = matcher
        self.log_sender = log_sender
        self.running = False
        self.active_users = active_users
        self.active_users_lock = active_users_lock
        self.frame_queue = Queue(maxsize=1) 
        self.unknowns = {}
        self.UNKNOWN_TIMEOUT = 5   # segundos sem ver → expira
        self.UNKNOWN_MATCH_THRESHOLD = 0.6
        self.unknown_seq = 0

        # {
        #   unknown_id: {
        #       "embedding": np.array,
        #       "quality": float,
        #       "last_seen": createdAt,
        #       "image_path": str
        #   }
        # }

    def start(self):
        self.running = True

        if DEBUG_VIEW:
            cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)
        else:
            print(self.url)
            # cap = cv2.VideoCapture(self.url)

            os.environ["OPENCV_FFMPEG_CAPTURE_OPTIONS"] = (
                "rtsp_transport;tcp|"
                "fflags;nobuffer|"
                "flags;low_delay|"
                "analyzeduration;1000000|"
                "probesize;1000000"
            )

            cap = cv2.VideoCapture(self.url, cv2.CAP_FFMPEG)
            cap.set(cv2.CAP_PROP_BUFFERSIZE, 2)

        if not cap.isOpened():
            print(f"[ERRO] Não abriu câmera {self.cameraId}")
            return

        print(f"[OK] Monitorando câmera: {self.cameraId}")

        capture_thread = Thread(
            target=self.capture_loop,
            args=(cap,),
            daemon=True
        )

        process_thread = Thread(
            target=self.processing_loop,
            daemon=True
        )

        capture_thread.start()
        process_thread.start()

        while self.running:
            time.sleep(1)

        cap.release()

    def stop(self):
        self.running = False

    def capture_loop(self, cap):
        fail_count = 0

        while self.running:
            ret, frame = cap.read()

            if not ret or frame is None:
                fail_count += 1
                if fail_count > 30:
                    print(f"[WARN] Reconectando câmera {self.cameraId}")
                    cap.release()
                    time.sleep(2)
                    cap.open(self.url)
                    fail_count = 0
                continue

            # sempre manter o frame mais recente
            if self.frame_queue.full():
                try:
                    self.frame_queue.get_nowait()
                except:
                    pass

            self.frame_queue.put(frame)

    def processing_loop(self):
        frame_count = 0

        while self.running:
            try:
                frame = self.frame_queue.get(timeout=1)
            except:
                continue

            frame_count += 1
            if frame_count % PROCESS_EVERY_N_FRAMES != 0:
                continue

            faces = self.face_model.get_faces(frame)


            for f in faces:
                emb = f.normed_embedding
                user_id, score = self.matcher.match(emb)

                if user_id is not None:
                    self.register_log(user_id, self.cameraId, self.sectorId, score)
                    continue 

                # Detecção de desconhecidos
                # x1, y1, x2, y2 = map(int, f.bbox)
                # h, w = frame.shape[:2]
                # x1, y1 = max(0, x1), max(0, y1)
                # x2, y2 = min(w, x2), min(h, y2)

                # face_crop = frame[y1:y2, x1:x2]

                # if face_crop.size == 0:
                #     continue

                # quality = self.face_quality(face_crop, frame.shape)
                
                # if quality < 100:   # threshold inicial simples
                #     continue

                # self.handle_unknown(face_crop, emb, quality, frame.shape)

            self.cleanup_unknowns()

                


    def register_log(self, personId, cameraId, sectorId, score):
        print(personId)
        print(cameraId)
        print(sectorId)
        print(score)

        with self.active_users_lock:
            personData = self.active_users.get(personId)

            if personData is None or personData['sectorId'] != sectorId:

                self.active_users[personId] = {
                    "cameraId": cameraId,
                    "sectorId": sectorId,
                    "score": score
                }

                self.log_sender.dotnet_create_environment_monitoring_log(
                    cameraId=cameraId,
                    personId=personId,
                    score=score,
                    createdAt=datetime.now(ZoneInfo("America/Sao_Paulo")).isoformat()
                )

    def save_face(self, face_img, user_id, score):
        ts = datetime.now().strftime("%Y%m%d_%H%M%S_%f")
        base_dir = f"faces/camera_{self.cameraId}/user_{user_id}"

        os.makedirs(base_dir, exist_ok=True)

        filename = f"{ts}_{score:.2f}.jpg"
        path = os.path.join(base_dir, filename)

        cv2.imwrite(path, face_img)

        print("Salvei imagem")


    def face_quality(self, face_img, frame_shape):
        gray = cv2.cvtColor(face_img, cv2.COLOR_BGR2GRAY)

        # Nitidez (Laplacian)
        blur_score = cv2.Laplacian(gray, cv2.CV_64F).var()

        # Tamanho relativo
        fh, fw = face_img.shape[:2]
        H, W = frame_shape[:2]
        size_ratio = (fw * fh) / (W * H)

        # pesos simples
        quality = (blur_score * 0.7) + (size_ratio * 1000 * 0.3)
        return quality
    
    def match_unknown(self, embedding):
        best_id = None
        best_score = 0

        for uid, data in self.unknowns.items():
            score = self.matcher.similarity(embedding, data["embedding"])
            if score > best_score:
                best_score = score
                best_id = uid

        if best_score >= self.UNKNOWN_MATCH_THRESHOLD:
            return best_id, best_score

        return None, None
    
    def handle_unknown(self, face_img, embedding, quality, frame_shape):
        now = time.time()

        unknown_id, score = self.match_unknown(embedding)

        # 🔁 Atualizar unknown existente
        if unknown_id:
            data = self.unknowns[unknown_id]

            if quality > data["quality"]:
                path = self.save_face(face_img, unknown_id, quality)
                data.update({
                    "embedding": embedding,
                    "quality": quality,
                    "image_path": path
                })

            data["last_seen"] = now
            return

        # 🆕 Criar novo unknown
        self.unknown_seq += 1
        uid = f"unknown_{self.unknown_seq}"

        path = self.save_face(face_img, uid, quality)

        self.unknowns[uid] = {
            "embedding": embedding,
            "quality": quality,
            "last_seen": now,
            "image_path": path
        }

    def cleanup_unknowns(self):
        now = time.time()
        to_remove = []

        for uid, data in self.unknowns.items():
            if now - data["last_seen"] > self.UNKNOWN_TIMEOUT:
                to_remove.append(uid)

        for uid in to_remove:
            del self.unknowns[uid]