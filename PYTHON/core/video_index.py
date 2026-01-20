import os
from datetime import datetime, timedelta
import threading

SEGMENT_SECONDS = 10  # 5 min

class VideoIndex:
    def __init__(self, base_dir="records"):
        self.base_dir = base_dir
        self.index = {}
        self.lock = threading.Lock()
        self.known_files = set()
        

    def build(self):
        with self.lock:
            self.index.clear()

            for cam in os.listdir(self.base_dir):
                cam_path = os.path.join(self.base_dir, cam)
                if not os.path.isdir(cam_path):
                    continue

                self.index[cam] = []
                print("Indexando câmera:", cam)

                for root, _, files in os.walk(cam_path):
                    for f in files:
                        if not f.endswith(".mkv"):
                            continue

                        try:
                            name = f.replace(".mkv", "")
                            _, date_part, time_part = name.split("_")
                            ts = f"{date_part}_{time_part}"
                            start = datetime.strptime(ts, "%Y%m%d_%H%M%S")
                        except:
                            continue

                        end = start + timedelta(seconds=SEGMENT_SECONDS)

                        self.index[cam].append({
                            "path": os.path.join(root, f),
                            "start": start,
                            "end": end
                        })

                self.index[cam].sort(key=lambda x: x["start"])

    def find_segments(self, camera_id, start_dt, duration):
        with self.lock:
            if camera_id not in self.index:
                raise Exception("Câmera não encontrada")

            end_dt = start_dt + timedelta(seconds=duration)

            segments = []
            for seg in self.index[camera_id]:
                if seg["end"] > start_dt and seg["start"] < end_dt:
                    segments.append(seg)

            if not segments:
                raise Exception("Nenhum segmento encontrado")

            return segments


    def update(self):
        with self.lock:
            for cam in os.listdir(self.base_dir):
                cam_path = os.path.join(self.base_dir, cam)
                if not os.path.isdir(cam_path):
                    continue

                self.index.setdefault(cam, [])

                for root, _, files in os.walk(cam_path):
                    for f in files:
                        if not f.endswith(".mkv"):
                            continue

                        full = os.path.join(root, f)
                        if full in self.known_files:
                            continue

                        try:
                            name = f.replace(".mkv", "")
                            _, date_part, time_part = name.split("_")
                            ts = f"{date_part}_{time_part}"
                            start = datetime.strptime(ts, "%Y%m%d_%H%M%S")
                        except:
                            continue

                        end = start + timedelta(seconds=SEGMENT_SECONDS)

                        self.index[cam].append({
                            "path": full,
                            "start": start,
                            "end": end
                        })

                        self.known_files.add(full)

                self.index[cam].sort(key=lambda x: x["start"])