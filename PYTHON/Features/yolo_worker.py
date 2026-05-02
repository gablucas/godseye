import os
import time

import cv2
from pika import frame
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Event
from ultralytics import YOLO
import supervision as sv


class YoloWorker(Process):
    """
    Process dedicado por câmera.

    Mantém uma instância YOLO com persist=True isolada, garantindo que
    os track_ids nunca colidam entre câmeras diferentes.

    Responsabilidades:
      - Detectar pessoas no frame
      - Manter tracking contínuo (track_id estável por câmera)
      - Enviar crops de face para o FaceWorker (face_queue)
      - Enviar posições/track_ids para o ZoneMonitor (zone_queue)
    """

    def __init__(self, camera, yolo_queue, face_queue, zone_queue):
        super().__init__(daemon=True)
        self.camera_id = camera["Id"]
        self.roi = camera.get("Roi") or []
        self.yolo_queue = yolo_queue
        self.face_queue = face_queue
        self.zone_queue = zone_queue
        self._stop_event = Event()

    def stop(self):
        self._stop_event.set()
        self.yolo_queue.put(None)

    def run(self):
        base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
        model_path = os.path.join(base_dir, "models", "yolov11m-face.pt")

        print(f"[YOLO] Iniciando YoloWorker para câmera {self.camera_id}")
        yolo = YOLO(model_path).to("cuda")
        print(f"[YOLO] Modelo carregado para câmera {self.camera_id}")

        # ✅ NOVO: controla tempo do último envio por track
        sent_to_face = {}  # track_id -> timestamp

        # 🔧 intervalo mínimo para reprocessar o mesmo track (em segundos)
        RETRY_INTERVAL = 2.0

        while not self._stop_event.is_set():
            try:
                frame = self.yolo_queue.get(timeout=1)
                # print(f"[YOLO cam {self.camera_id}] recebi frame shape {frame.shape}")
            except:
                # print(f"[YOLO cam {self.camera_id}] yolo_queue vazia")
                continue

            if frame is None:
                break

            try:
                results = yolo.track(
                    source=frame,
                    persist=True,
                    tracker="botsort.yaml",
                    classes=[0],
                    verbose=False,
                )
                # print(f"[YOLO cam {self.camera_id}] track concluído — boxes: {len(results[0].boxes)}")
            except Exception as e:
                import traceback
                # print(f"[YOLO cam {self.camera_id}] ERRO no track:")
                traceback.print_exc()
                continue

            detections = sv.Detections.from_ultralytics(results[0])

            if detections.tracker_id is None:
                tracker_ids = []
                boxes = []
            else:
                tracker_ids = detections.tracker_id.tolist()
                boxes = detections.xyxy.tolist()

            # 1. Zone monitor (mantém igual)
            # if not self.zone_queue.full():
            #     self.zone_queue.put_nowait({
            #         "camera_id": self.camera_id,
            #         "tracker_ids": tracker_ids,
            #         "boxes": boxes,
            #     })

            current_ids = set(tracker_ids)
            now = time.time()

            # 2. Envio para FaceWorker (COM RETRY INTELIGENTE)
            for i, track_id in enumerate(tracker_ids):

                # print(f"👤 Câmera {self.camera_id} detectou pessoa com track_id {track_id}")

                # ❌ ANTIGO (remove isso)
                # if track_id in sent_to_face:
                #     continue

                # ✅ NOVO: só envia se passou tempo suficiente
                # last_sent = sent_to_face.get(track_id)

                # if last_sent is not None and (now - last_sent) < RETRY_INTERVAL:
                #     continue

                x1, y1, x2, y2 = [int(v) for v in boxes[i]]

                h, w = frame.shape[:2]

                # Calcular margem (ex: 20% do tamanho da face)
                face_w = x2 - x1
                face_h = y2 - y1
                margin_x = int(face_w * 0.2)  # 20% de margem horizontal
                margin_y = int(face_h * 0.2)  # 20% de margem vertical

                # Expandir com margem
                x1 = x1 - margin_x
                y1 = y1 - margin_y
                x2 = x2 + margin_x
                y2 = y2 + margin_y

                # Clamp para não sair do frame
                x1 = max(0, x1)
                y1 = max(0, y1)
                x2 = min(w, x2)
                y2 = min(h, y2)

                if x2 <= x1 or y2 <= y1:
                    continue

                crop = frame[y1:y2, x1:x2]

                if crop.size == 0:
                    continue

                print(f"👤 Câmera {self.camera_id} enviando crop para FaceWorker — track_id {track_id}, crop shape {crop.shape}")

                # Extrai ROI de face (RoiType=1) para esta câmera, se existir
                face_roi = None
                face_roi_data = next((r for r in self.roi if r["RoiType"] == 1), None)

                if face_roi_data:
                    coords = face_roi_data["Coordinates"]
                    face_roi = {
                        "min_width": coords["Width"] * w,
                        "min_height": coords["Height"] * h,
                    }

                    if face_roi:
                        crop_height = crop.shape[0]  # altura
                        crop_width = crop.shape[1]   # largura

                        # print(f"COMPRIMENTO DO ROI {face_roi['min_width']}");
                        # print(f"COMPRIMENTO DO CROP {crop_width}");

                        # if crop_width < face_roi["min_width"]:
                        #     continue


                try:
                    self.face_queue.put_nowait((self.camera_id, track_id, crop, face_roi))
                    sent_to_face[track_id] = now
                except:
                    print(f"[YOLO] face_queue CHEIA — cam {self.camera_id}")

            # 3. Limpeza de tracks que sumiram
            # ❌ ANTIGO
            # lost = sent_to_face - current_ids
            # sent_to_face -= lost

            # ✅ NOVO (dict)
            lost = [tid for tid in sent_to_face.keys() if tid not in current_ids]
            for tid in lost:
                sent_to_face.pop(tid, None)