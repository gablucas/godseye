import numpy as np

from infrastructure.logger import LogSender
from services.face_matcher_service import FaceMatcher
from services.monitoring_service import validate_monitoring_data
from application.monitor_manager import MonitorManager

def start_monitoring_system(app, face_model):
    video_index = app.state.video_index
    data = app.state.godseye.get_all()
    monitor_manager = MonitorManager()

    cameras, persons = validate_monitoring_data(data)

    ids = []
    embeddings = []

    for p in persons:
        ids.append(p["Id"])
        embeddings.append(p["Embedding"])

    emb_matrix = np.asarray(embeddings, dtype=np.float32)

    matcher = FaceMatcher(
        ids=ids,
        emb_matrix=emb_matrix
    )

    log_sender = LogSender(
        "https://localhost:7010/api/EnvironmentMonitoring"
    )

    monitor_manager.start_monitoring(
        face_model,
        cameras,
        matcher,
        log_sender
    )

    # background tasks (se ainda fizer sentido)
    # if not app.state.background_started:
    #     video_index.build()

    #     threading.Thread(
    #         target=cleanup_old_videos,
    #         args=("records", 7, video_index),
    #         daemon=True
    #     ).start()

    #     app.state.background_started = True
