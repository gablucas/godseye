# infrastructure/ffmpeg_hls.py
import subprocess
import os
from config.settings import STREAM_DIR

def start_hls_stream(stream_name: str, rtsp_url: str):
    output_path = os.path.join(STREAM_DIR, stream_name)
    os.makedirs(output_path, exist_ok=True)

    cmd = [
        "ffmpeg",

        # Força RTSP estável
        "-rtsp_transport", "tcp",

        # Entrada
        "-i", rtsp_url,

        # Re-encode para corrigir timestamps ausentes
        "-c:v", "libx264",
        "-preset", "veryfast",
        "-tune", "zerolatency",

        # Áudio
        "-c:a", "aac",
        "-ar", "44100",
        "-b:a", "96k",

        # Formato HLS
        "-f", "hls",
        "-hls_time", "1",
        "-hls_list_size", "4",
        "-hls_flags", "delete_segments+append_list",

        # Saída
        f"{output_path}/index.m3u8"
    ]

    return subprocess.Popen(cmd)
