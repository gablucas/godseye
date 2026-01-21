import subprocess
import numpy as np

class RecognitionVideoService:
    def __init__(self, video_path, face_model: FaceModel):
        self.video_path = video_path
        self.face_model = face_model

    def start_recognize(self):
        for frame in self._read_frames():
            self._process_frame(frame)

    def _read_frames(self):
        command = [
            "ffmpeg",
            "-i", self.video_path,
            "-vf", f"fps={self.fps}",
            "-f", "rawvideo",
            "-pix_fmt", "rgb24",
            "-"
        ]

        process = subprocess.Popen(
            command,
            stdout=subprocess.PIPE,
            stderr=subprocess.DEVNULL,
            bufsize=10**8
        )

        frame_size = self.width * self.height * 3

        while True:
            raw_frame = process.stdout.read(frame_size)
            if not raw_frame:
                break

            frame = np.frombuffer(raw_frame, np.uint8)
            frame = frame.reshape((self.height, self.width, 3))
            yield frame

    def _process_frame(self, frame):
        # IA entra aqui
        print("Frame recebido:", frame.shape)