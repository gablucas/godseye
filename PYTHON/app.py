from fastapi import FastAPI
from contextlib import asynccontextmanager
from fastapi.middleware.cors import CORSMiddleware

from api.face import router as face_router
from api.monitor import router as monitor_router
from api.stream import router as stream_router
from api.clip import router as clip_router

from core.godseyedata import GodsEyeData
from core.godseyedata_loader import load_godseye_data_from_api
from core.start_system import start_monitoring_system
from core.video_index import VideoIndex
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model


# ======================================================
# LIFESPAN
# ======================================================
@asynccontextmanager
async def lifespan(app: FastAPI):
    # ===== STARTUP =====
    app.state.godseye = GodsEyeData()
    app.state.video_index = VideoIndex()
    app.state.background_started = False

    try:
        # PEGA OS DADOS INICIAIS
        data = await load_godseye_data_from_api()
        app.state.godseye.set(data)

        # INICIA O SISTEMA
        face_model = get_face_model()
        start_monitoring_system(app, face_model)

        print("Monitoramento iniciado automaticamente")

    except Exception as e:
        print("Erro ao iniciar monitoramento:", e)

    yield

    print("Monitoramento encerrado")


# ======================================================
# APP
# ======================================================
app = FastAPI(lifespan=lifespan)


# ======================================================
# ESTADO GLOBAL (sem I/O)
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
