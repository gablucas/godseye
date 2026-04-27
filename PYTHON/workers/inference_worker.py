import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Queue
from services.face_recognition_service import FaceModel
from ultralytics import YOLO
import supervision as sv


class InferenceWorker(Process):
    def __init__(self, inference_queue: Queue, result_queue: Queue):
        super().__init__(daemon=True)
        self.inference_queue = inference_queue
        self.result_queue = result_queue

    def run(self):
        yolo = YOLO("yolo26s.pt").to("cuda")
        face_model = FaceModel()

        while True:
            try:
                item = self.inference_queue.get(timeout=1)
            except:
                continue

            if item is None:
                break

            camera_id, frame = item

            results = yolo.track(
                source=frame,
                persist=True,
                tracker="botsort.yaml",
                classes=[0],
                verbose=False
            )

            detections = sv.Detections.from_ultralytics(results[0])

            if detections.tracker_id is None:
                tracker_ids = []
                boxes = []
            else:
                tracker_ids = detections.tracker_id.tolist()
                boxes = detections.xyxy.tolist()

            embeddings_by_track = {}
            for i, track_id in enumerate(tracker_ids):
                x1, y1, x2, y2 = [int(v) for v in boxes[i]]
                crop = frame[y1:y2, x1:x2]
                if crop.size == 0:
                    continue

                faces = face_model.get_faces(crop)
                if not faces:
                    continue

                embeddings_by_track[track_id] = [
                    {
                        "embedding": face.normed_embedding.tolist(),
                        "bbox": face.bbox.tolist()
                    }
                    for face in faces
                ]

            self.result_queue.put((camera_id, tracker_ids, boxes, embeddings_by_track))