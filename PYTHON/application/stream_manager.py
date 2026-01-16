# stream/manager.py
import os
import signal
from fastapi import HTTPException
from infrastructure.ffmpeg_hls import start_hls_stream, STREAM_DIR

streams = {}  # stream_name -> subprocess

class StreamAlreadyRunning(Exception):
    pass

class StreamNotFound(Exception):
    pass

class StreamManager:
    def __init__(self):
        self.streams: dict[str, any] = {}

    def start_stream(self, name: str, rtsp_url: str) -> dict:
        if name in self.streams:
            return {
                "message": "Stream já iniciada",
                "url": f"/stream/{name}/index.m3u8",
            }

        proc = start_hls_stream(name, rtsp_url)
        self.streams[name] = proc

        return {
            "message": "Stream iniciado",
            "url": f"/stream/{name}/index.m3u8",
        }

    def stop_stream(self, name: str) -> dict:
        if name not in self.streams:
            raise StreamNotFound("Stream não encontrado")

        proc = self.streams.pop(name)
        proc.send_signal(signal.SIGTERM)

        return {"message": "Stream parado"}

    def get_stream_file(self, name: str, filename: str) -> str:
        file_path = os.path.join(STREAM_DIR, name, filename)

        if not os.path.exists(file_path):
            raise StreamNotFound("Arquivo não encontrado")

        return file_path


# def start_stream(name: str, rtsp_url: str):
#     if name in streams:
#         raise HTTPException(400, f"Stream '{name}' já está ativo.")

#     proc = start_hls_stream(name, rtsp_url)
#     streams[name] = proc

#     return {
#         "message": "Stream iniciado",
#         "url": f"/stream/{name}/index.m3u8"
#     }

# def stop_stream(name: str):
#     if name not in streams:
#         raise HTTPException(404, "Stream não encontrado")

#     proc = streams.pop(name)
#     proc.send_signal(signal.SIGTERM)

#     return {"message": "Stream parado"}

# def get_stream_file(name: str, filename: str):
#     file_path = os.path.join(STREAM_DIR, name, filename)
#     if not os.path.exists(file_path):
#         raise HTTPException(404, "Arquivo não encontrado")
#     return file_path
