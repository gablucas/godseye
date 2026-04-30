import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Event
import time


class ZoneMonitor(Process):
    """
    Process por câmera.

    Recebe posições e track_ids do YoloWorker e decide se uma pessoa
    entrou em alguma zona de interesse (ROI) ou está há tempo demais
    (dwell time).

    Separado do reconhecimento facial — não precisa esperar o InsightFace.
    """

    DWELL_THRESHOLD_SECONDS = 30  # tempo para disparar alerta de permanência

    def __init__(self, camera_id, roi, zone_queue):
        super().__init__(daemon=True)
        self.camera_id = camera_id
        self.roi = roi or []
        self.zone_queue = zone_queue
        self._stop_event = Event()

        # track_id -> timestamp de entrada na zona
        self.dwell_start = {}

    def stop(self):
        self._stop_event.set()

    def run(self):
        restricted_zones = [r for r in self.roi if r.get("RoiType") == 2]

        while not self._stop_event.is_set():
            try:
                item = self.zone_queue.get(timeout=0.5)
            except:
                continue

            if item["camera_id"] != self.camera_id:
                # Devolve para outro ZoneMonitor
                if not self.zone_queue.full():
                    self.zone_queue.put_nowait(item)
                continue

            tracker_ids = item["tracker_ids"]
            boxes = item["boxes"]
            now = time.time()

            for i, track_id in enumerate(tracker_ids):
                box = boxes[i]
                cx, cy = self._center(box)

                in_zone = any(
                    self._point_in_zone(cx, cy, zone)
                    for zone in restricted_zones
                )

                if in_zone:
                    if track_id not in self.dwell_start:
                        self.dwell_start[track_id] = now
                        self._on_zone_entry(track_id, box)

                    elapsed = now - self.dwell_start[track_id]
                    if elapsed > self.DWELL_THRESHOLD_SECONDS:
                        self._on_dwell_exceeded(track_id, elapsed, box)
                        self.dwell_start[track_id] = now  # reseta timer
                else:
                    if track_id in self.dwell_start:
                        self.dwell_start.pop(track_id)

            # Limpa tracks que saíram da cena
            current = set(tracker_ids)
            for tid in list(self.dwell_start.keys()):
                if tid not in current:
                    self.dwell_start.pop(tid)

    def _center(self, box):
        x1, y1, x2, y2 = box
        return (x1 + x2) / 2, (y1 + y2) / 2

    def _point_in_zone(self, cx, cy, zone):
        coords = zone.get("Coordinates", {})
        zx = coords.get("X", 0) * 640
        zy = coords.get("Y", 0) * 360
        zw = coords.get("Width", 0) * 640
        zh = coords.get("Height", 0) * 360
        return zx <= cx <= zx + zw and zy <= cy <= zy + zh

    def _on_zone_entry(self, track_id, box):
        print(f"🚨 Câmera {self.camera_id} — track {track_id} entrou em zona restrita")
        # TODO: dispara webhook, alerta, etc.

    def _on_dwell_exceeded(self, track_id, elapsed, box):
        print(f"⏱ Câmera {self.camera_id} — track {track_id} na zona há {elapsed:.0f}s")
        # TODO: dispara alerta de permanência excessiva