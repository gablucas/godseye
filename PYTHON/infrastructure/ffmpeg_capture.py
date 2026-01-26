import subprocess
import os

def ffmpeg_capture(
    rtsp_url: str,
    fps: float = 1.0,
    width: int = 1280,
    height: int = 720,
    cameraId: int = 0,
    environment_monitoring: bool = False,
    dwell_time_monitoring: bool = False,
    record: bool = False,
    record_path: str | None = None
):
    command = [
        "ffmpeg",
        "-loglevel", "error",
        "-rtsp_transport", "tcp",
        "-i", rtsp_url,
    ]

    # ────── GRAVAÇÃO ──────
    if record:
        os.makedirs(record_path, exist_ok=True)
        command += [
            "-map", "0:v",
            "-c:v", "copy",
            "-f", "segment",
            "-segment_time", "10",
            "-reset_timestamps", "1",
            "-strftime", "1",
            f"{record_path}/{cameraId}_%Y%m%d_%H%M%S.mkv"
        ]

    # ────── MONITORAMENTO (RAWVIDEO) ──────
    if environment_monitoring or dwell_time_monitoring:
        command += [
            "-map", "0:v",
            "-vf", f"fps={fps},scale={width}:{height}",
            "-f", "rawvideo",
            "-pix_fmt", "bgr24",
            "pipe:1"
        ]

        stdout = subprocess.PIPE
    else:
        # ⚠️ CRÍTICO: sem rawvideo, NÃO pode ser PIPE
        stdout = subprocess.DEVNULL

    return subprocess.Popen(
        command,
        stdout=stdout,
        stderr=subprocess.DEVNULL,
        bufsize=10**8
    )
