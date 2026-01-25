from pydantic import BaseModel
from datetime import datetime

class DwellTimeMonitoringCreateRequest(BaseModel):
    personId: str
    cameraId: str
    FirstSeen: datetime 