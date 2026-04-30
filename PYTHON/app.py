import asyncio
from fastapi import FastAPI
from contextlib import asynccontextmanager
from fastapi.middleware.cors import CORSMiddleware
import numpy as np

from api.face import router as face_router
from api.monitor import router as monitor_router
from api.stream import router as stream_router
from api.clip import router as clip_router
from api.video import router as video_router

from Features.monitor_manager import MonitorManager
from core.godseyedata import GodsEyeData
from core.video_index import VideoIndex
from contextlib import asynccontextmanager
from fastapi import FastAPI
from core.startup_retry import load_godseye_with_retry

# LIFESPAN
@asynccontextmanager
async def lifespan(app: FastAPI):
    # app.state.godseye = GodsEyeData()
    app.state.video_index = VideoIndex()
    app.state.background_started = False
    app.state.init_lock = asyncio.Lock()
    app.state.monitor_manager = MonitorManager()

    asyncio.create_task(load_godseye_with_retry(app))
    
    yield  # app rodando aqui
    
    # tudo abaixo do yield roda no shutdown
    app.state.monitor_manager.stop_all()

app = FastAPI(lifespan=lifespan)

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
app.include_router(video_router)
app.include_router(face_router, prefix="/api")
app.include_router(monitor_router, prefix="/api")
app.include_router(stream_router, prefix="/api")
app.include_router(clip_router, prefix="/api")
