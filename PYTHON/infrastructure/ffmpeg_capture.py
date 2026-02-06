import subprocess
import os

def ffmpeg_capture(
    rtsp_url: str,
    fps: float = 10.0,
    width: int = 640,
    height: int = 360,
    cameraId: int = 0,
    features: dict = {},
    record_path: str | None = None
):
    command = [
        "ffmpeg",
        "-loglevel", "error",
        "-rtsp_transport", "tcp",
        "-hwaccel", "cuda",
        "-reorder_queue_size", "4000",
        "-i", rtsp_url,
    ]

    print(features)

    # ────── GRAVAÇÃO ──────
    if features.get("incident_recording", False) and record_path and cameraId:
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
    if features.get("environment_monitoring", False) or features.get("dwell_time_monitoring", False):
        command += [
            "-map", "0:v",
            "-vf", f"fps={fps},scale={width}:-2",
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
        # stderr=subprocess.DEVNULL,
        stderr=None, #Mostra os erros no console
        bufsize=10**8
    )