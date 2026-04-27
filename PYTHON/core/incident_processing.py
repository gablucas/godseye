import json
from os import path
from pathlib import Path
import httpx
import asyncio
from datetime import datetime, timedelta
from schemas.incident_schema import IncidentResponse, PersonSeen, IncidentRecordingUpdateRequest
from services.clip_service import ClipService
from services.face_recognition_service import FaceModel
import cv2


FACE_REPROCESS_TTL = 2.0      # segundos
VIDEO_POLL_INTERVAL = 1.0    # segundos
SKIP_N_FRAMES = 10          # pula frames para acelerar processamento

class IncidentLoadError(Exception):
    pass

class ProcessingIncident():
    def __init__(self, clip_service: ClipService):
        self.clip_service = clip_service

    async def run(self):
        self.init_models()

        while True:
            print("############# INICIANDO SISTEMA DE INCIDENTE")

            try:
                result = await self.process_incident()
                if not result:
                    await asyncio.sleep(10)
                    continue

                incident, file_name, video_path = result

                # ⚠️ processamento pesado → thread
                persons = await asyncio.to_thread(
                    self.process_video,
                    video_path
                )

                update_request = IncidentRecordingUpdateRequest(
                    id=incident.id,
                    persons=list(persons.values()),
                    file_name=file_name
                )

                await self.done_incident(update_request)
                print("#FINALIZANDO INCIDENTE")

            except IncidentLoadError as e:
                print("Erro ao processar incidente:", e)

            await asyncio.sleep(10)


    def init_models(self):
        self.face_model = FaceModel()

    async def process_incident(self) -> tuple[IncidentResponse, str, str] | None:
        print("BUSCANDON INCIDENTES")
        incident = await self.get_incident()

        if incident is None:
            return None

        file_name, output_path = await asyncio.to_thread(
            self.clip_service.generate_event_clip,
            incident.camera_id,
            incident.incident_time
        )
        
        return incident, file_name, output_path


    async def done_incident(self, request: IncidentRecordingUpdateRequest):
        print(request)
        async with httpx.AsyncClient(verify=False, timeout=10) as client:
            response = await client.put(
                "https://localhost:7010/api/incidentrecording/process/done",
                json=request.model_dump(by_alias=True, mode="json")
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
        
    def process_video(self, video_path: str) -> dict[int, PersonSeen]:
        persons = {}

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

            video_seconds = frame_index / fps  # ✅ TEMPO REAL NO VÍDEO

            if frame_index % SKIP_N_FRAMES == 0:
                
                timestamp = video_start_time + timedelta(seconds=video_seconds)

                print(f"[FRAME] {video_path.name} frame={frame_index} t={video_seconds:.2f}s")
                self.process_frame(
                    frame=frame,
                    timestamp=timestamp,
                    video_seconds=video_seconds,
                    persons=persons
                )

            frame_index += 1

        cap.release()
        return persons

    def process_frame(self, frame, timestamp, video_seconds, persons):
        faces = self.face_model.get_faces(frame)
        if not faces:
            return

        for face in faces:

            emb = face.normed_embedding
            self.process_embedding(emb, timestamp,video_seconds, persons)

    def process_embedding(self, emb, timestamp, video_seconds, persons):
        person_id, score = self.face_matcher.match(emb)

        if person_id is None:
            return

        self.register_log(person_id, score, timestamp, video_seconds, persons)

    def register_log(self, person_id, score, timestamp, video_seconds, persons):
        if person_id in persons:
            return
        
        persons[person_id] = PersonSeen(
            id=person_id,
            seen_at=timestamp.isoformat(),
            video_offset_seconds=video_seconds
        )


    def parse_video_start_time(self, video_path: Path | str) -> datetime:
        video_path = Path(video_path)

        _, date_part, time_part = video_path.stem.split("_")

        return datetime.strptime(
            f"{date_part}_{time_part}",
            "%Y%m%d_%H%M%S"
        )