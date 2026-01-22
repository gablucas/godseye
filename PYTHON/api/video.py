import os
from fastapi import APIRouter, HTTPException
from fastapi.responses import FileResponse

router = APIRouter()


@router.get("/videos/{filename}")
def get_video(filename: str):
    path = f"clips/{filename}"

    if not os.path.exists(path):
        raise HTTPException(status_code=404, detail="Vídeo não encontrado")

    return FileResponse(
        path,
        media_type="video/x-matroska",
        filename=filename
    )