import asyncio
import numpy as np
from fastapi import FastAPI
import asyncio
from fastapi import FastAPI
from contextlib import asynccontextmanager
import numpy as np

from core.godseyedata_loader import load_godseye_data_from_api
from core.incident_processing import ProcessingIncident
from services.clip_service import ClipService
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model

async def initialize_monitoring(app: FastAPI, data):
    async with app.state.init_lock:
        if app.state.background_started:
            return

        app.state.background_started = True

        cameras = validate_monitoring_data(data)
        monitor_manager = app.state.monitor_manager

        for cam in cameras:
            monitor_manager.add_camera(cam)

        clip_service = ClipService(app.state.video_index)

        # incident_processor = ProcessingIncident(
        #     clip_service=clip_service
        # )

        # loop = asyncio.get_running_loop()
        # loop.create_task(incident_processor.run())