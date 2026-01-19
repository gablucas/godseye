from pydantic import BaseModel
from datetime import datetime

class ClipRequest(BaseModel):
    cameraId: str
    dateTime: datetime