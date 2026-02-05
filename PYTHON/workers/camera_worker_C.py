# stream/camera_worker.py

import cv2
import time
import os
from threading import Thread
from queue import Queue
from datetime import datetime
from zoneinfo import ZoneInfo
from ultralytics import YOLO
import supervision as sv

from infrastructure.log_queue import log_queue
from schemas.dwell_time_monitoring import DwellTimeMonitoringCreateRequest
from schemas.environment_monitoring_schema import EnvironmentMonitoringCreateRequest
from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender
import numpy as np
from infrastructure.ffmpeg_capture import ffmpeg_capture

TIMEOUT = 5
DEBUG_VIEW = False
PROCESS_EVERY_N_FRAMES = 1


class CameraWorker:
    def __init__(
        self,
        face_model,
        cameraId,
        rtsp_url,
        sectorId,
        matcher: FaceMatcher,
        active_users,
        active_users_lock,
        capture_fps: float = 15.0,
        width: int = 800,
        height: int = 600,
        environment_monitoring: bool = False,
        incident_recording: bool = False,
        dwell_time_monitoring: bool = False
    ):
        self.face_model = face_model
        self.cameraId = cameraId
        self.url = rtsp_url
        self.sectorId = sectorId
        self.matcher = matcher
        self.environment_monitoring = environment_monitoring
        self.incident_recording = incident_recording
        self.dwell_time_monitoring = dwell_time_monitoring

        self.running = False
        self.active_users = active_users
        self.active_users_lock = active_users_lock

        self.frame_queue = Queue(maxsize=1)

        self.capture_fps = capture_fps
        self.width = width
        self.height = height
        self.frame_size = width * height * 3

        self.unknowns = {}
        self.UNKNOWN_TIMEOUT = 5
        self.UNKNOWN_MATCH_THRESHOLD = 0.6
        self.unknown_seq = 0
        self.last_sent_time = 0.0
        self.SEND_INTERVAL = 1.0
        self.yolo = YOLO("yolo26s.pt")
        self.yolo.to("cuda")

        self.box_annotator = sv.BoxAnnotator()
        self.label_annotator = sv.LabelAnnotator()

        # Mapeamento track_id -> person_id
        self.track_to_person = {}

        # Controle de tracks ativos
        self.active_tracks = {}
        self.TRACK_TIMEOUT = 3  # segundos sem aparecer
        self.counter = 0

    def start(self):
        self.running = True

        # print("########################################")
        # print(f"Iniciando servico com a camera {self.cameraId}")

        capture_thread = Thread(
            target=self.capture_loop,
            daemon=True
        )

        process_thread = Thread(
            target=self.processing_loop,
            daemon=True
        )

        capture_thread.start()
        process_thread.start()

        # while self.running:
        #     time.sleep(1)

    def stop(self):
        self.running = False

    def capture_loop(self):
        while self.running:

            # Se nenhuma funcionalidade estiver ativa, não cria FFmpeg
            if not self.environment_monitoring and not self.incident_recording and not self.dwell_time_monitoring:
                time.sleep(1)
                continue

            process = ffmpeg_capture(
                rtsp_url=self.url,
                fps=self.capture_fps,
                width=self.width,
                height=self.height,
                cameraId=self.cameraId,
                environment_monitoring=self.environment_monitoring,
                dwell_time_monitoring=self.dwell_time_monitoring,
                record=self.incident_recording,
                record_path=f"records/{self.cameraId}"
            )

            try:
                while self.running:

                    if not self.environment_monitoring and not self.dwell_time_monitoring:
                        time.sleep(0.2)
                        continue

                    # print(f"[DEBUG] Aguardando frame da camera {self.cameraId}...")
                    raw = process.stdout.read(self.frame_size)
                    # print(f"[DEBUG] Frame recebido ({len(raw)} bytes)")

                    if len(raw) != self.frame_size:
                        # print(f"[WARN] Frame incompleto (cam {self.cameraId}), reiniciando FFmpeg")
                        break

                    frame = np.frombuffer(raw, np.uint8).reshape(
                        (self.height, self.width, 3)
                    )

                    # ===============================
                    # 🔥 RECORTE A PARTIR DA LINHA
                    # ===============================
                    # p1 = (0, 330)
                    # p2 = (self.width - 1, 325)

                    # h, w, _ = frame.shape

                    # mask = np.zeros((h, w), dtype=np.uint8)

                    # polygon = np.array([
                    #     p1,
                    #     p2,
                    #     (w - 1, h - 1),
                    #     (0, h - 1)
                    # ])

                    # cv2.fillPoly(mask, [polygon], 255)

                    # frame = cv2.bitwise_and(frame, frame, mask=mask)
                    # ===============================

                    now = time.time()

                    if now - self.last_sent_time >= self.SEND_INTERVAL:
                        self.last_sent_time = now
                    

                        if self.frame_queue.full():
                            try:
                                self.frame_queue.get_nowait()
                            except:
                                pass

                        self.frame_queue.put(frame)

            finally:
                process.kill()
                time.sleep(2)  # evita loop agressivo

    def processing_loop(self):
        while self.running and (self.environment_monitoring or self.dwell_time_monitoring):
            try:
                frame = self.frame_queue.get(timeout=1)
            except:
                continue

            now = time.time()

            # Processa frame com YOLO
            results = self.yolo.track(
                source=frame,
                persist=True,
                tracker="botsort.yaml",
                classes=[0],
                verbose=False
            )

            # Conjunto de track_ids atuais
            result = results[0]
            detections = sv.Detections.from_ultralytics(result)

            current_track_ids = set()

            # Se não tiver nenhum track_id, pula
            if detections.tracker_id is None:
                continue


            else:
                tracker_ids = detections.tracker_id.tolist()
                current_track_ids.update(tracker_ids)

                # Itera sobre cada detecção
                for i, track_id in enumerate(tracker_ids):
                    x1, y1, x2, y2 = detections.xyxy[i].astype(int)
                    
                    # Atualiza a lista de tracks ativos
                    self.active_tracks[track_id] = now

                    # Se track_id já mapeado, registra o log da pessoa vinculada
                    if track_id in self.track_to_person:
                        user_id = self.track_to_person[track_id]
                        self.register_log(
                            user_id,
                            self.cameraId,
                            self.sectorId,
                            score=None
                        )
                        continue

                    # Recorta o frame do track_id não mapeado
                    crop = frame[y1:y2, x1:x2]
                    if crop.size == 0:
                        continue

                    cv2.imwrite(f"crops/face_{track_id}.jpg", crop)

                    # Detecta as faces utilizando o insight face
                    faces = self.face_model.get_faces(crop)
                    if not faces:
                        continue
                    
                    # Gera o embedding e faz o match
                    emb = faces[0].normed_embedding
                    user_id, score = self.matcher.match(emb)

                    print(f"[DEBUG] Face processada - Track {track_id}")

                    # Se encontrou pessoa, inicia o registro do log
                    if user_id is not None:

                        # Mapeia o track_id para o user_id
                        self.track_to_person[track_id] = user_id
                        self.register_log(
                            user_id,
                            self.cameraId,
                            self.sectorId,
                            score
                        )

                # 🔻 cleanup de tracks que sumiram
                self.cleanup_tracks(now, current_track_ids)
      
    def register_log(self, personId, cameraId, sectorId, score):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        with self.active_users_lock:
            personData = self.active_users.get(personId)

            # ENVIRONMENT MONITORING
            if (self.environment_monitoring and (personData is None or personData["sector_id"] != sectorId)):

                print("REGISTRANDO LOG")
                environmentMonitoring = EnvironmentMonitoringCreateRequest(
                    camera_id=cameraId,
                    person_id=personId,
                    score=score
                )
                
                log_queue.put((
                    LogSender.dotnet_create_environment_monitoring_log,
                    environmentMonitoring
                ))

            # DWELL TIME MONITORING
            if (self.dwell_time_monitoring):
                if (personData is None):
                    # print("USUARIO NÃO EXISTE")

                    dwellTimeMonitoring = DwellTimeMonitoringCreateRequest(
                        camera_id=cameraId,
                        person_id=personId,
                        first_seen=datetime.now(ZoneInfo("America/Sao_Paulo")).isoformat()
                    )

                    log_queue.put((
                        LogSender.dotnet_create_dwell_time_monitoring_log,
                        dwellTimeMonitoring
                    ))

                else:
                    diff_last_seen = (now - personData["last_seen"]).total_seconds() / 60
                    diff_created = (now - personData["created_at"]).total_seconds() / 60

                    # print(f"ULTIMA HORA VISTO: {diff_last_seen}")
                    # print(f"TEMPO NA CAMARA: {diff_created}")

                    if (diff_created >= 100):
                        log_queue.put((
                            LogSender.dotnet_send_timeout_alert,
                            {
                                "person_id": personId,
                                "camera_id": cameraId,
                            }
                        ))

                    if (diff_last_seen >= 5):
                        log_queue.put((
                            LogSender.dotnet_update_last_seen,
                            {
                                "person_id": personId,
                                "camera_id": cameraId,
                                "last_seen": now.isoformat()
                            }
                        ))


            if personData is None:
                self.active_users[personId] = {
                    "camera_id": cameraId,
                    "sector_id": sectorId,
                    "score": score,
                    "created_at": now,
                    "last_seen": now,
                    "updated_at": now
                }

            else:
                # ATUALIZAR DADOS LISTA USERS
                personData["last_seen"] = now
                personData["updated_at"] = now
                personData["camera_id"] = cameraId
                personData["sector_id"] = sectorId
                personData["score"] = score

    def cleanup_tracks(self, now, current_track_ids):

        # Identifica tracks que sumiram - set difference
        # {1,2,3,4} - {2,3} = {1,4}
        lost_tracks = set(self.active_tracks.keys()) - current_track_ids

        # Itera sobre cada track perdido
        for track_id in lost_tracks:
            person_id = self.track_to_person.get(track_id)

            # Remove track das listas
            self.active_tracks.pop(track_id, None)
            self.track_to_person.pop(track_id, None)

            if person_id is not None:
                self.on_person_left(person_id)


    def on_person_left(self, person_id):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        with self.active_users_lock:
            person = self.active_users.get(person_id)
            if not person:
                return

            # Atualiza last_seen FINAL
            person["last_seen"] = now
            person["updated_at"] = now

            # Aqui você pode:
            # - fechar sessão
            # - calcular dwell final
            # - enviar evento "saiu"
            # - marcar status = inactive