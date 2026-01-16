import requests
import numpy as np
import json
from services.monitoring_service import validate_monitoring_data
from services.face_matcher_service import FaceMatcher
from infrastructure.logger import LogSender
from test.camera_work_test import CameraWorkerTest
from services.face_recognition_service import FaceModel
from threading import Thread, Lock

active_users = {}
active_users_lock = Lock()


result = requests.get(
    "https://localhost:7010/api/godseye",
    verify=False
).json()


result_data = result.get("dados")
raw_data = result_data["data"]

if isinstance(raw_data, str):
    raw_data = json.loads(raw_data)


cameras, persons = validate_monitoring_data(raw_data)

ids = []
embeddings = []

for p in persons:
    ids.append(p["Id"])
    embeddings.append(p["Embedding"])

# converte para matriz numpy
emb_matrix = np.asarray(embeddings, dtype=np.float32)

matcher = FaceMatcher(
    ids=ids,
    emb_matrix=emb_matrix
)

log_sender = LogSender("https://localhost:7010/api/logger")

face_model = FaceModel()

for cam in cameras:
    camWork = CameraWorkerTest(face_model, cam["Id"], cam['Connection'], cam['SectorId'], matcher, log_sender, active_users, active_users_lock)
    camWork.start()

