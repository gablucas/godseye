# stream/camera_worker.py
from multiprocessing import Process, Queue, Event
from threading import Thread, Lock


import time
from datetime import datetime
from turtle import width
from zoneinfo import ZoneInfo
import cv2
import cv2
from ultralytics import YOLO
import supervision as sv
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


class CameraProcess(Process):
    def __init__(
        self,
        camera_id,
        rtsp_url,
        sector_id,
        face_matcher: FaceMatcher,
        features=None,
        log_queue=None,
        shared_person=None
    ):
        super().__init__()
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.sector_id = sector_id
        self.matcher = face_matcher
        self.features = features or {}
        self.stop_event = Event()
        self.log_queue = log_queue
        self.shared_person = shared_person or {}
        self.frame_queue = Queue(maxsize=1)
        self.width = 640
        self.height = 360
        

    def stop(self):
        self.stop_event.set()

    def run(self):
        self.frame_lock = Lock()
        self.init_models()

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

        self.person_on_frame_by_track_id = {}
        self.active_tracks = {}

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

        if self.features.get("environment_monitoring", False) == True or self.features.get("dwell_time_monitoring", False) == True:

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

                    if self.frame_queue.full():
                        try:
                            self.frame_queue.get_nowait()
                        except:
                            pass

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
            # Tem gente (tracks ativos), mas o movimento de pixels é baixo
            # Mantemos um FPS seguro para não perder o ID se eles levantarem
            self.last_detection = now
            self.target_fps = FPS_STEADY 
            print(f"👀 MODO STEADY (Pessoas paradas)")
            
        elif IS_LOW_MOTION or (now - self.last_detection < COOLDOWN_TIME):
            # Movimento residual (vento, luz mudando) ou cooldown
            self.target_fps = FPS_STEADY
            
        else:
            # Deserto total
            self.target_fps = FPS_IDLE
            print(f"💤 MODO IDLE")

        return motion_pixels > 500

    def inference_loop(self):
        while not self.stop_event.is_set():
            now = time.time()

            frame = None
            while not self.frame_queue.empty():
                try:
                    frame = self.frame_queue.get_nowait()
                except:
                    break

            if frame is None:
                continue

            self.movement_detection(frame, now)

            print(f"Target FPS: {self.target_fps:.2f}, Active Tracks: {len(self.active_tracks)}, Queue Size: {self.frame_queue.qsize()}")
            # CONTROLE DE FPS
            frame_interval = 1.0 / self.target_fps
            elapsed = now - self.last_run

            if elapsed < frame_interval:
                time.sleep(frame_interval - elapsed)

            self.last_run = time.time()
            print("PROCESSANDO FRAME")

            # INFERÊNCIA
            self.process_frame(frame)


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
            detections.tracker_id = np.array([]) # Garante consistência para o loop
        else:
            tracker_ids_on_frame = detections.tracker_id.tolist()

        # ==================================================
        # 1. PROCESSAR TRACKS ATIVOS (Se houver alguém)
        # ==================================================
        if len(tracker_ids_on_frame) > 0:
            for i, track_id in enumerate(tracker_ids_on_frame):
                x1, y1, x2, y2 = detections.xyxy[i].astype(int)

                # Atualiza timestamp do track (para manter FPS alto)
                self.active_tracks[track_id] = now

                # CASO A: JÁ TEM PESSOA ASSOCIADA A ESSE TRACK
                if track_id in self.person_on_frame_by_track_id:
                    person_id = self.person_on_frame_by_track_id[track_id]

                    events = self.evaluate_rules(
                        personId=person_id,
                        cameraId=self.camera_id,
                        sectorId=self.sector_id,
                        score=None
                    )

                    for event in events:
                        self.dispatch_log(event)

                    self.update_shared_person(
                        personId=person_id,
                        cameraId=self.camera_id,
                        sectorId=self.sector_id,
                        score=None
                    )
                    continue

                # CASO B: NOVA PESSOA (TENTAR RECONHECIMENTO)
                crop = frame[y1:y2, x1:x2]
                if crop.size == 0:
                    continue

                faces = self.face_model.get_faces(crop)
                if not faces:
                    continue

                emb = faces[0].normed_embedding
                person_id, score = self.matcher.match(emb)

                if person_id is None:
                    continue

                # Associa track -> pessoa
                self.person_on_frame_by_track_id[track_id] = person_id
                print(f"🔗 Associando track {track_id} à pessoa {person_id} (score: {score:.2f})")

                events = self.evaluate_rules(
                    personId=person_id,
                    cameraId=self.camera_id,
                    sectorId=self.sector_id,
                    score=score
                )

                for event in events:
                    self.dispatch_log(event)

                self.update_shared_person(
                    personId=person_id,
                    cameraId=self.camera_id,
                    sectorId=self.sector_id,
                    score=score
                )

        # ==================================================
        # 2. LIMPEZA DE TRACKS PERDIDOS (A Parte Crítica)
        # ==================================================
        # Identifica tracks que estão em 'active_tracks' mas NÃO vieram no YOLO agora
        current_tracks_set = set(tracker_ids_on_frame)
        
        # Pega as chaves existentes antes de iterar para evitar erro de tamanho do dicionário mudando
        known_tracks = list(self.active_tracks.keys())

        for tid in known_tracks:
            if tid not in current_tracks_set:
                print(f"🧹 Limpando Track {tid} (Saiu de cena)")
                
                # Remove do controle de FPS
                self.active_tracks.pop(tid, None)
                
                # Remove da associação Track -> Pessoa (para evitar inconsistência se o ID for reutilizado)
                self.person_on_frame_by_track_id.pop(tid, None)

    def evaluate_rules(self, personId, cameraId, sectorId, score):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))
        personData = self.shared_person.get(personId)

        events = []

        # =========================
        # ENVIRONMENT MONITORING
        # =========================
        if self.features.get("environment_monitoring", False):
            if personData is None or personData["sector_id"] != sectorId:
                events.append({
                    "type": "environment_enter",
                    "person_id": personId,
                    "camera_id": cameraId,
                    "score": score
                })

        # =========================
        # DWELL TIME
        # =========================
        if self.features.get("dwell_time_monitoring", False):
            if personData is None:
                events.append({
                    "type": "dwell_start",
                    "person_id": personId,
                    "camera_id": cameraId,
                    "first_seen": now
                })
            else:
                diff_last_seen = (now - personData["last_seen"]).total_seconds() / 60
                diff_created = (now - personData["created_at"]).total_seconds() / 60

                if diff_created >= 100:
                    events.append({
                        "type": "timeout_alert",
                        "person_id": personId,
                        "camera_id": cameraId
                    })

                if diff_last_seen >= 5:
                    events.append({
                        "type": "update_last_seen",
                        "person_id": personId,
                        "camera_id": cameraId,
                        "last_seen": now
                    })

        return events

    def dispatch_log(self, event):
        if self.log_queue is None:
            return
        
        match event["type"]:

            case "environment_enter":
                self.log_queue.put((
                    LogSender.dotnet_create_environment_monitoring_log,
                    EnvironmentMonitoringCreateRequest(
                        camera_id=event["camera_id"],
                        person_id=event["person_id"],
                        score=event["score"]
                    )
                ))

            case "dwell_start":
                self.log_queue.put((
                    LogSender.dotnet_create_dwell_time_monitoring_log,
                    DwellTimeMonitoringCreateRequest(
                        camera_id=event["camera_id"],
                        person_id=event["person_id"],
                        first_seen=event["first_seen"].isoformat()
                    )
                ))

            case "timeout_alert":
                self.log_queue.put((
                    LogSender.dotnet_send_timeout_alert,
                    event
                ))

            case "update_last_seen":
                self.log_queue.put((
                    LogSender.dotnet_update_last_seen,
                    event
                ))

    def update_shared_person(self, personId, cameraId, sectorId, score):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        # Pega uma cópia dos dados atuais (ou None se não existir)
        # IMPORTANTE: Isso aqui retorna um dicionário puro, desconectado do Manager
        personData = self.shared_person.get(personId)

        if personData is None:
            # CRIANDO: Aqui funciona normal porque você está atribuindo na chave
            self.shared_person[personId] = {
                "camera_id": cameraId,
                "sector_id": sectorId,
                "score": score,
                "created_at": now,
                "last_seen": now,
                "updated_at": now
            }
        else:
            # ATUALIZANDO: 
            # 1. Atualize a sua cópia local
            personData.update({
                "camera_id": cameraId,
                "sector_id": sectorId,
                "score": score,
                "last_seen": now,
                "updated_at": now
            })
            
            # 2. O PULO DO GATO: Reescreva o dicionário inteiro de volta na chave compartilhada
            # Sem essa linha, os outros processos nunca vão ver a atualização!
            self.shared_person[personId] = personData
