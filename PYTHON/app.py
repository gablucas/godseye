import asyncio
from fastapi import FastAPI
from contextlib import asynccontextmanager
from fastapi.middleware.cors import CORSMiddleware
import numpy as np

from api.face import router as face_router
from api.monitor import router as monitor_router
from api.stream import router as stream_router
from api.clip import router as clip_router
from api.video import router as video_router

from core.godseyedata import GodsEyeData
from core.godseyedata_loader import load_godseye_data_from_api
from core.video_index import VideoIndex
from core.incident_processing import ProcessingIncident
from application.monitor_manager import MonitorManager

from domain.data_validation import MonitoringDataError
from infrastructure.log_queue import start_log_worker
from services.clip_service import ClipService
from services.face_matcher_service import FaceMatcher
from services.face_processor_service import FaceRecognitionProcessor
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model

from contextlib import asynccontextmanager
from fastapi import FastAPI
from core.startup_retry import load_godseye_with_retry

# LIFESPAN
@asynccontextmanager
async def lifespan(app: FastAPI):
    app.state.godseye = GodsEyeData()
    app.state.video_index = VideoIndex()
    app.state.background_started = False

    start_log_worker()

    asyncio.create_task(load_godseye_with_retry(app))
    yield

app = FastAPI(lifespan=lifespan)

# ======================================================
# CORS
# ======================================================
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# ======================================================
# ROTAS
# ======================================================
app.include_router(video_router)
app.include_router(face_router, prefix="/api")
app.include_router(monitor_router, prefix="/api")
app.include_router(stream_router, prefix="/api")
app.include_router(clip_router, prefix="/api")
