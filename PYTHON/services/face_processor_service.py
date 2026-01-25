
from services.face_recognition_service import FaceModel
from services.face_matcher_service import FaceMatcher

class FaceRecognitionProcessor:
    def __init__(self, face_model: FaceModel, face_matcher: FaceMatcher):
        self.face_model = face_model
        self.matcher = face_matcher

    def process_frame(self, frame):
        results = []

        faces = self.face_model.get_faces(frame)

        if not faces:
                    return

        for f in faces:
            emb = f.normed_embedding
            user_id, score = self.matcher.match(emb)

            if user_id is not None:
                results.append({
                    "user_id": user_id,
                    "score": score
                })

        return results
