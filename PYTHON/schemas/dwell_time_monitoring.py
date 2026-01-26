from pydantic import BaseModel, Field
from datetime import datetime

class DwellTimeMonitoringCreateRequest(BaseModel):
    person_id: int = Field(alias="personId")
    camera_id: int = Field(alias="cameraId")
    first_seen: datetime = Field(alias="firstSeen")

    model_config = {
        "populate_by_name": True
    }