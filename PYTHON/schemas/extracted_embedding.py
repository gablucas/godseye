from typing import List
from pydantic import BaseModel, Field
from datetime import datetime

class ExtractedEmbedding(BaseModel):
    camera_id: int = Field(alias="CameraId")
    embedding: List[float] = Field(alias="Embedding")
    identified_at: datetime = Field(alias="IdentifiedAt")

    model_config = {
        "populate_by_name": True
    }