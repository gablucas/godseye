from fastapi import APIRouter, HTTPException
from fastapi.responses import FileResponse

from application.stream_manager import (
    StreamManager,
    StreamAlreadyRunning,
    StreamNotFound,
)

router = APIRouter()
stream_manager = StreamManager()


@router.post("/camera/start")
def start_stream(name: str, rtsp_url: str):
    try:
        return stream_manager.start_stream(name, rtsp_url)

    except StreamAlreadyRunning as e:
        raise HTTPException(status_code=400, detail=str(e))

    except Exception:
        raise HTTPException(status_code=500, detail="Erro ao iniciar stream")


@router.post("/camera/stop")
def stop_stream(name: str):
    try:
        return stream_manager.stop_stream(name)

    except StreamNotFound as e:
        raise HTTPException(status_code=404, detail=str(e))

    except Exception:
        raise HTTPException(status_code=500, detail="Erro ao parar stream")


@router.get("/stream/{name}/{file}")
def get_stream(name: str, file: str):
    try:
        path = stream_manager.get_stream_file(name, file)

        response = FileResponse(path)

        # 🔥 necessário para WebAssembly / HLS
        response.headers["Access-Control-Allow-Origin"] = "*"
        response.headers["Access-Control-Allow-Headers"] = "*"
        response.headers["Access-Control-Allow-Methods"] = "*"

        return response

    except StreamNotFound as e:
        raise HTTPException(status_code=404, detail=str(e))
