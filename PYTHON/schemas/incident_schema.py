from pydantic import BaseModel, Field
from datetime import datetime

class IncidentResponse(BaseModel):
    id: int = Field(alias="id")
    camera_id: int = Field(alias="cameraId")
    incident_time: datetime = Field(alias="incidentTime")

class PersonSeen(BaseModel):
    id: int = Field(alias="personId")
    seen_at: datetime = Field(alias="seenAt")
    video_offset_seconds: float = Field(alias="videoOffsetSeconds")

    model_config = {
        "populate_by_name": True
    }

class UpdateIncidentRequest(BaseModel):
    id: int = Field(alias="incidentId")
    persons: list[PersonSeen] = Field(alias="persons")
    file_name: str = Field(alias="fileName")

    model_config = {
        "populate_by_name": True
    }