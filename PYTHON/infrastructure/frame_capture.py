from infrastructure.ffmpeg_capture import ffmpeg_capture
import numpy as np

class FrameCapture:
    def __init__(
        self,
        input_source: str,
        fps: float,
        width: int,
        height: int,
        enable_frames: bool = True,
        enable_recording: bool = False,
        record_path: str | None = None
    ):
        self.input_source = input_source
        self.process = None
        self.frame_size = width * height * 3
        self.enable_frames = enable_frames
        self.enable_recording = enable_recording

    def start(self):
        self.process = ffmpeg_capture(
            rtsp_url=self.input_source,
            fps=2,
            width=1280,
            height=720,
            environment_monitoring=self.enable_frames,
            record=self.enable_recording,
            record_path="records"
        )

    def read_frame(self):
        if not self.enable_frames:
            return None

        raw = self.process.stdout.read(self.frame_size)
        if len(raw) != self.frame_size:
            return None

        return np.frombuffer(raw, np.uint8)
    

    
    def stop(self):
        self.running = False

        if self.process:
            try:
                self.process.kill()
            except:
                pass
