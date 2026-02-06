# stream/camera_worker.py
import time
from datetime import datetime
from zoneinfo import ZoneInfo
from ultralytics import YOLO
import supervision as sv
from multiprocessing import Process, Queue, Event

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
        capture_fps=15,
        width=800,
        height=600,
        features={},
        log_queue=None
    ):
        super().__init__()
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.sector_id = sector_id
        self.matcher = face_matcher
        self.capture_fps = capture_fps
        self.width = width
        self.height = height
        self.features = features or {}
        self.stop_event = Event()
        self.log_queue = log_queue

    def stop(self):
        self.stop_event.set()

    def run(self):
        self.init_models()
        self.run_stream_loop()

    def init_models(self):
        self.yolo = YOLO("yolo26s.pt")
        self.yolo.to("cuda")
        self.face_model = FaceModel()

        self.box_annotator = sv.BoxAnnotator()
        self.label_annotator = sv.LabelAnnotator()

        self.track_to_person = {}
        self.active_tracks = {}
        self.active_users = {}

    def run_stream_loop(self):
        frame_size = self.width * self.height * 3

        process = ffmpeg_capture(
            rtsp_url=self.rtsp_url,
            fps=self.capture_fps,
            width=self.width,
            height=self.height,
            cameraId=self.camera_id,
            features=self.features,
            record_path=f"records/{self.camera_id}"
        )

        try:
            while not self.stop_event.is_set():
                raw = process.stdout.read(frame_size)
                if (len(raw) != frame_size):
                    break

                frame = np.frombuffer(raw, np.uint8).reshape(
                    (self.height, self.width, 3)
                )

                self.process_frame(frame)
        finally:
            process.kill()

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
            return

        tracker_ids = detections.tracker_id.tolist()
        
        for i, track_id in enumerate(tracker_ids):
            x1, y1, x2, y2 = detections.xyxy[i].astype(int)

            self.active_tracks[track_id] = now

            # ============================
            # JÁ TEM PESSOA ASSOCIADA
            # ============================
            if track_id in self.track_to_person:
                person_id = self.track_to_person[track_id]

                events = self.evaluate_rules(
                    personId=person_id,
                    cameraId=self.camera_id,
                    sectorId=self.sector_id,
                    score=None
                )

                for event in events:
                    self.dispatch_log(event)

                self.update_active_users(
                    personId=person_id,
                    cameraId=self.camera_id,
                    sectorId=self.sector_id,
                    score=None
                )
                continue

            # ============================
            # NOVA PESSOA (REID)
            # ============================
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

            # associa track -> pessoa
            self.track_to_person[track_id] = person_id

            events = self.evaluate_rules(
                personId=person_id,
                cameraId=self.camera_id,
                sectorId=self.sector_id,
                score=score
            )

            for event in events:
                self.dispatch_log(event)

            self.update_active_users(
                personId=person_id,
                cameraId=self.camera_id,
                sectorId=self.sector_id,
                score=score
            )

    def evaluate_rules(self, personId, cameraId, sectorId, score):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))
        personData = self.active_users.get(personId)

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

    def update_active_users(self, personId, cameraId, sectorId, score):
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        personData = self.active_users.get(personId)

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
            personData.update({
                "camera_id": cameraId,
                "sector_id": sectorId,
                "score": score,
                "last_seen": now,
                "updated_at": now
            })
