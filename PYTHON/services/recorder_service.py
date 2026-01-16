import subprocess
import os

class CameraRecorder:
    def __init__(self, camera_id, rtsp_url, output_dir):
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.output_dir = output_dir
        self.process = None

    def start(self):
        os.makedirs(self.output_dir, exist_ok=True)

        cmd = [
            "ffmpeg",
            "-loglevel", "error",
            "-rtsp_transport", "tcp",
            "-i", self.rtsp_url,

            "-c", "copy",

            "-f", "segment",
            "-segment_time", "10",
            "-segment_atclocktime", "1",
            "-reset_timestamps", "1",
            "-strftime", "1",

            f"{self.output_dir}/{self.camera_id}_%Y%m%d_%H%M%S.mkv"
        ]

        self.process = subprocess.Popen(
            cmd,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL
        )

    def stop(self):
        if self.process:
            self.process.terminate()