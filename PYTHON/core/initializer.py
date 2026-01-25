import asyncio
import numpy as np
from fastapi import FastAPI
import asyncio
from fastapi import FastAPI
from contextlib import asynccontextmanager
import numpy as np

from core.godseyedata_loader import load_godseye_data_from_api

from core.incident_processing import ProcessingIncident
from application.monitor_manager import MonitorManager

from infrastructure.log_queue import start_log_worker
from services.clip_service import ClipService
from services.face_matcher_service import FaceMatcher
from services.face_processor_service import FaceRecognitionProcessor
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model

def initialize_monitoring(app: FastAPI, data):
    if getattr(app.state, "background_started", False):
        return

    print("###### INICIANDO INITIALIZE")
    app.state.background_started = True

    cameras, persons = validate_monitoring_data(data)

    ids = [p["Id"] for p in persons]
    embeddings = [p["Embedding"] for p in persons]

    face_matcher = FaceMatcher(
        ids=ids,
        emb_matrix=np.asarray(embeddings, dtype=np.float32)
    )
    print("###### FACEMODEL")
    face_model = get_face_model()


    print("###### FaceRecognitionProcessor")
    face_processor = FaceRecognitionProcessor(
        face_model=face_model,
        face_matcher=face_matcher
    )

    app.state.face_model = face_model
    app.state.face_matcher = face_matcher
    app.state.face_processor = face_processor

    app.state.godseye.set(data)
    app.state.video_index.build()

    print("###### MonitorManager")
    monitor_manager = MonitorManager(face_model, face_matcher, cameras)
    monitor_manager.start_monitoring_async()

    print("######INICIANDO CLIP SERVICE")
    clip_service = ClipService(app.state.video_index)

    incident_processor = ProcessingIncident(
        clip_service=clip_service,
        face_model=face_model,
        face_matcher=face_matcher
    )

    print("🚀 Registrando Incident Processor no event loop")
    loop = asyncio.get_running_loop()
    loop.create_task(incident_processor.start())
