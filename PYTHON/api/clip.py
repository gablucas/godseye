from fastapi import APIRouter, HTTPException, Request
from services.clip_service import ClipService

router = APIRouter()


@router.post("/clip/{camera_id}")
def gerar_clip(camera_id: str, request: Request):
    try:
        video_index = request.app.state.video_index
        service = ClipService(video_index)

        path = service.generate_event_clip(camera_id)

        return {
            "status": "ok",
            "clip": path
        }

    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))
