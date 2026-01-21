import cv2

class ClipFrameReader:
    def __init__(self, path: str, skip_n_frames: int = 10):
        self.cap = cv2.VideoCapture(path)
        if not self.cap.isOpened():
            raise RuntimeError(f"Não foi possível abrir o vídeo: {path}")

        self.fps = self.cap.get(cv2.CAP_PROP_FPS) or 30.0
        self.frame_index = 0
        self.skip_n_frames = skip_n_frames

    def read_frame(self):
        while True:
            ret, frame = self.cap.read()
            if not ret:
                return None

            if self.frame_index % self.skip_n_frames == 0:
                timestamp_ms = self.cap.get(cv2.CAP_PROP_POS_MSEC)

                if timestamp_ms > 0:
                    video_time = timestamp_ms / 1000.0
                else:
                    video_time = self.frame_index / self.fps

                self.frame_index += 1
                return frame, video_time

            self.frame_index += 1

    def release(self):
        self.cap.release()
