import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Queue
from services.face_recognition_service import FaceModel
from ultralytics import YOLO
import supervision as sv
import numpy as np
import time


class InferenceWorker(Process):
    def __init__(self, inference_queue: Queue, result_queue: Queue,
                 batch_size: int = 8, batch_timeout: float = 0.05):
        super().__init__(daemon=True)
        self.inference_queue = inference_queue
        self.result_queue = result_queue
        self.batch_size = batch_size
        self.batch_timeout = batch_timeout  # espera até 50ms para montar o batch

    def run(self):
        # TensorRT engine em vez de .pt
        yolo = YOLO("yolo26s.engine").to("cuda")
        face_model = FaceModel()

        while True:
            batch_frames, batch_camera_ids = self._collect_batch()
            if not batch_frames:
                continue

            self._process_batch(yolo, face_model, batch_frames, batch_camera_ids)

    def _collect_batch(self):
        """Acumula frames até atingir batch_size ou timeout."""
        batch_frames = []
        batch_camera_ids = []
        deadline = time.time() + self.batch_timeout

        while len(batch_frames) < self.batch_size:
            timeout = max(0, deadline - time.time())
            try:
                item = self.inference_queue.get(timeout=timeout)
            except:
                break  # timeout estourou, processa o que tem

            if item is None:
                break

            camera_id, frame = item

            # Garante numpy HWC uint8 (compatível com YOLO e face_model)
            if not isinstance(frame, np.ndarray):
                frame = frame.cpu().numpy()  # caso venha como tensor GPU futuramente

            batch_frames.append(frame)
            batch_camera_ids.append(camera_id)

        return batch_frames, batch_camera_ids

    def _process_batch(self, yolo, face_model, batch_frames, batch_camera_ids):
        """Roda YOLO em batch e depois extrai faces por detecção."""

        # Inferência em batch — muito mais eficiente que frame por frame
        results_list = yolo.track(
            source=batch_frames,
            persist=False,
            tracker="botsort.yaml",
            classes=[0],
            verbose=False
        )

        for camera_id, frame, results in zip(batch_camera_ids, batch_frames, results_list):
            detections = sv.Detections.from_ultralytics(results)

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