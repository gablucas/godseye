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

from services.clip_service import ClipService
from services.face_matcher_service import FaceMatcher
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model

async def initialize_monitoring(app: FastAPI, data):
    async with app.state.init_lock:
        if app.state.background_started:
            return

        app.state.background_started = True

        cameras, persons = validate_monitoring_data(data)
        face_matcher = build_face_matcher(persons)

        monitor_manager = MonitorManager(cameras, face_matcher)
        monitor_manager.start_monitoring()

        clip_service = ClipService(app.state.video_index)

        incident_processor = ProcessingIncident(
            clip_service=clip_service,
            face_matcher=face_matcher
        )

        loop = asyncio.get_running_loop()
        loop.create_task(incident_processor.run())


def build_face_matcher(persons):
    ids = [p["Id"] for p in persons]
    embeddings = np.asarray(
        [p["Embedding"] for p in persons],
        dtype=np.float32
    )

    return FaceMatcher(ids=ids, emb_matrix=embeddings)