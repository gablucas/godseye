from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from api.face import router as face_router
from api.monitor import router as monitor_router
from api.stream import router as stream_router
from api.clip import router as clip_router

from core.video_index import VideoIndex

app = FastAPI()

# ======================================================
# ESTADO GLOBAL DA APLICAÇÃO
# ======================================================
app.state.video_index = VideoIndex()
app.state.background_started = False


# ======================================================
# CORS
# ======================================================
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ======================================================
# ROTAS
# ======================================================
app.include_router(face_router, prefix="/api")
app.include_router(monitor_router, prefix="/api")
app.include_router(stream_router, prefix="/api")
app.include_router(clip_router, prefix="/api")
