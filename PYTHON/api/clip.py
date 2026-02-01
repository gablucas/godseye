from fastapi import APIRouter, HTTPException, Request
from services.clip_service import ClipService
from schemas.clip_schema import ClipRequest

router = APIRouter()

@router.post("/clip/{camera_id}")
def gerar_clip(data: ClipRequest, request: Request):
    try:
        video_index = request.app.state.video_index
        service = ClipService(video_index)

        path = service.generate_event_clip(
            camera_id=data.cameraId,
            event_time=data.dateTime
        )

        return {
            "status": "ok",
            "clip": path
        }

    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))
