from pydantic import BaseModel, Field
from decimal import Decimal

class EnvironmentMonitoringCreateRequest(BaseModel):
    camera_id: int = Field(alias="cameraId")
    person_id: int = Field(alias="personId")
    score: Decimal = Field(alias="score")

    model_config = {
        "populate_by_name": True
    }