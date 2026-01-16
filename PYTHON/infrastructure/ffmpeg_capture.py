import subprocess

def ffmpeg_capture(rtsp_url: str, fps: float = 1.0, width: int = 1280, height: int = 720):
    command = [
        "ffmpeg",
        "-rtsp_transport", "tcp",
        "-i", rtsp_url,

        # força resolução (importante para rawvideo)
        "-s", f"{width}x{height}",

        # filtro de FPS (economia real de CPU)
        "-vf", f"fps={fps}",

        "-f", "rawvideo",
        "-pix_fmt", "bgr24",
        "-"
    ]

    return subprocess.Popen(
        command,
        stdout=subprocess.PIPE,
        stderr=subprocess.DEVNULL,
        bufsize=10**8
    )
