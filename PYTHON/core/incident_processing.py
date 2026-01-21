import json
from os import path
from pathlib import Path
import httpx
import asyncio
from datetime import datetime, timedelta
from infrastructure.clip_frame_reader import ClipFrameReader
from infrastructure.frame_capture import FrameCapture
from schemas.incident_schema import IncidentResponse, UpdateIncidentRequest
from services.clip_service import ClipService
from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher
from services.face_processor_service import FaceRecognitionProcessor
import cv2


FACE_REPROCESS_TTL = 2.0      # segundos
VIDEO_POLL_INTERVAL = 1.0    # segundos
SKIP_N_FRAMES = 10          # pula frames para acelerar processamento

class IncidentLoadError(Exception):
    pass

class ProcessingIncident():
    def __init__(self, clip_service: ClipService, face_model: FaceModel, face_matcher: FaceMatcher):
        self.clip_service = clip_service
        self.face_model = face_model
        self.face_matcher = face_matcher
        self.persons: list = []

    async def start(self):
        while True:
            try:
                result = await self.process_incident()
                if not result:
                    await asyncio.sleep(10)
                    continue

                incident, path = result

                self.process_video(video_path=path)

                update_request = UpdateIncidentRequest(id=incident.id, person_ids=self.persons, video_path=str(path))
                print(update_request)
                
                await self.done_incident(update_request)
                print("#FINALIZANDO INCIDENTE")

            except IncidentLoadError as e:
                print("Erro ao processar incidente:", e)

            await asyncio.sleep(10)


    async def process_incident(self):
        incident = await self.get_incident()
        if not incident:
            return

        path = await asyncio.to_thread(
            self.clip_service.generate_event_clip,
            incident.camera_id,
            incident.incident_time
        )
        
        return incident, path


    async def done_incident(self, request: UpdateIncidentRequest):
        async with httpx.AsyncClient(verify=False, timeout=10) as client:
            response = await client.put(
                "https://localhost:7010/api/incidentrecording/process/done",
                json=request.model_dump(by_alias=True)
            )

            response.raise_for_status()

    async def get_incident(self) -> IncidentResponse | None:

        print("#TEMPORARIO - Buscando incidente para processar...")
        async with httpx.AsyncClient(verify=False, timeout=10) as client:
            response = await client.get(
                "https://localhost:7010/api/incidentrecording/process"
            )

        response.raise_for_status()
        result = response.json()


        if result.get("success") is not True:
            raise IncidentLoadError(
                "Erro ao pegar os dados do incidente"
            )
        
        result_data = result.get("data")

        if not result_data:
            return None

        incident = IncidentResponse.model_validate(result_data)

        print("INCIDENTE CONVERTIDO")
        print(incident)

        return incident
        
    def process_video(self, video_path: str):
        video_path = Path(video_path)

        cap = cv2.VideoCapture(str(video_path))

        if not cap.isOpened():
            print(f"[ERRO] Falha ao abrir {video_path}")
            return
        
        fps = cap.get(cv2.CAP_PROP_FPS)
        if fps <= 0:
            fps = 30.0


        video_start_time = self.parse_video_start_time(video_path)
        frame_index = 0

        while True:
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

    def process_frame(self, frame, video_time, timestamp):
        faces = self.face_model.get_faces(frame)
        if not faces:
            return

        for face in faces:

            emb = face.normed_embedding
            self.process_embedding(emb, timestamp)

    def process_embedding(self, emb, timestamp):
        person_id, score = self.face_matcher.match(emb)

        if person_id is None:
            return

        self.register_log(person_id, score, timestamp)

    def register_log(self, person_id, score, timestamp):
        if person_id in self.persons:
            return

        self.persons.append(person_id)


    def parse_video_start_time(self, video_path: Path | str) -> datetime:
        video_path = Path(video_path)

        _, date_part, time_part = video_path.stem.split("_")

        return datetime.strptime(
            f"{date_part}_{time_part}",
            "%Y%m%d_%H%M%S"
        )