import json
import httpx
import asyncio
from datetime import timedelta
from schemas.incident_schema import IncidentResponse, UpdateIncidentRequest
from services.clip_service import ClipService

class IncidentLoadError(Exception):
    pass

class ProcessingIncident():
    def __init__(self, clip_service: ClipService):
        self.clip_service = clip_service

    async def start(self):
        while True:
            try:
                incident = await self.process_incident()

                
                if incident:
                    await self.done_incident(incident)

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
            response = await client.post(
                "https://localhost:7010/api/incident/done",
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