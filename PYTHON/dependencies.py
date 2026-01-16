# dependencies.py
from services.face_recognition_service import FaceModel

_face_model: FaceModel | None = None

def get_face_model() -> FaceModel:
    global _face_model
    if _face_model is None:
        print("[INFO] Carregando FaceModel (singleton)")
        _face_model = FaceModel()
    return _face_model
