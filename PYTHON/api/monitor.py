from fastapi import APIRouter, HTTPException, Depends, Request
import requests
import json
import numpy as np
import threading

from dependencies import get_face_model
from services.face_recognition_service import FaceModel
from services.monitoring_service import validate_monitoring_data
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender
from Features.monitor_manager import MonitorManager

from infrastructure.recorder_registry import start_recorder
from core.video_cleanup import cleanup_old_videos

router = APIRouter()

# monitor_manager = MonitorManager()


# @router.post("/monitor/start")
# def start_monitoring(request: Request, face_model: FaceModel = Depends(get_face_model)):
#     video_index = request.app.state.video_index
#     background_started = request.app.state.background_started

#     result = requests.get(
#         "https://localhost:7010/api/godseye",
#         verify=False
#     ).json()

#     if result.get("sucesso") is not True:
#         raise HTTPException(
#             status_code=400,
#             detail="Não foi possível buscar os dados para monitoramento"
#         )
    
#     result_data = result.get("dados")

#     if not result_data or "data" not in result_data:
#         raise HTTPException(
#             status_code=400,
#             detail="Resposta inválida da API (.NET): campo 'data' ausente"
#         )

#     raw_data = result_data["data"]

#     if isinstance(raw_data, str):
#         try:
#             raw_data = json.loads(raw_data)
#         except json.JSONDecodeError:
#             raise HTTPException(
#                 status_code=400,
#                 detail="Campo 'data' não é um JSON válido"
#             )
#     try:
#         cameras, persons = validate_monitoring_data(raw_data)
#     except ValueError as e:
#         raise HTTPException(status_code=400, detail=str(e))
    
#     ids = []
#     embeddings = []

#     for p in persons:
#         ids.append(p["Id"])
#         embeddings.append(p["Embedding"])

#     # converte para matriz numpy
#     emb_matrix = np.asarray(embeddings, dtype=np.float32)

#     matcher = FaceMatcher(
#         ids=ids,
#         emb_matrix=emb_matrix
#     )

#     log_sender = LogSender("https://localhost:7010/api/EnvironmentMonitoring")

#     monitor_manager.start_monitoring(face_model, cameras, matcher, log_sender)

    # for cam in cameras:
    #     start_recorder(
    #         camera_id=cam["Id"],
    #         rtsp_url=cam["Connection"]
    #     )

    # if not background_started:
    #     video_index.build()

    #     threading.Thread(\
    #         target=cleanup_old_videos,
    #         args=("records", 7, video_index),
    #         daemon=True
    #     ).start()

    #     request.app.state.background_started = True

    # return {"status": "monitoring started"}


@router.post("/monitor/stop")
def stop_monitoring():
    monitor_manager.stop_all()
    return {"status": "monitoring stopped"}
