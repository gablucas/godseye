from pydantic import BaseModel, Field
from datetime import datetime

class IncidentResponse(BaseModel):
    id: int = Field(alias="id")
    camera_id: int = Field(alias="cameraId")
    incident_time: datetime = Field(alias="incidentTime")

class PersonSeen(BaseModel):
    id: int = Field(alias="personId")
    first_seen: datetime = Field(alias="firstSeen")

    model_config = {
        "populate_by_name": True
    }

class UpdateIncidentRequest(BaseModel):
    id: int = Field(alias="incidentId")
    persons: list[PersonSeen] = Field(alias="persons")
    video_path: str = Field(alias="videoPath")

    model_config = {
        "populate_by_name": True
    }