from pydantic import BaseModel, Field
from datetime import datetime

class IncidentResponse(BaseModel):
    id: int = Field(alias="id")
    camera_id: int = Field(alias="cameraId")
    incident_time: datetime = Field(alias="incidentTime")

class UpdateIncidentRequest(BaseModel):
    id: int = Field(alias="incidentId")
    person_ids: list[int] = Field(alias="personIds")
    video_path: str = Field(alias="videoPath")

    model_config = {
        "populate_by_name": True
    }