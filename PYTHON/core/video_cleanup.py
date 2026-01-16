import os
import time
from datetime import datetime, timedelta

def cleanup_old_videos(base_dir, keep_days, index):
    while True:
        cutoff = datetime.now() - timedelta(days=keep_days)

        for cam in os.listdir(base_dir):
            cam_path = os.path.join(base_dir, cam)
            if not os.path.isdir(cam_path):
                continue

            for root, _, files in os.walk(cam_path):
                for f in files:
                    if not f.endswith(".mkv"):
                        continue

                    try:
                        name = f.replace(".mkv", "")
                        _, date_part, time_part = name.split("_")
                        ts = f"{date_part}_{time_part}"
                        dt = datetime.strptime(ts, "%Y%m%d_%H%M%S")
                    except:
                        continue

                    if dt < cutoff:
                        os.remove(os.path.join(root, f))

        index.build()
        time.sleep(6 * 3600)  # a cada 6h
