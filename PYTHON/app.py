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
from core.incident_processing import ProcessingIncident

from services.clip_service import ClipService
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
        
        print("#1 - Buscando dados do GodsEye...")
        data = await load_godseye_data_from_api()
        print("#1 - Requisição concluída.")

        print("#2 - Vinculado dados ao estado global...")
        app.state.godseye.set(data)
        print("#2 - Dados vinculados.")

        app.state.video_index.build()

        print("\n========== VIDEO INDEX ANTES ==========")
        for cam_id, segments in app.state.video_index.index.items():
            print(f"\n📷 CÂMERA: {cam_id}")
            print(f"   Total de segmentos: {len(segments)}")

            for seg in segments[-5:]:  # mostra só os últimos 5
                print(
                    f"   ▶ {seg['start']} -> {seg['end']} | {seg['path']}"
                )
        print("=================================\n")
        
        print("#3 - Instanciando modelo de reconhecimento facial...")
        face_model = get_face_model()
        print("#3 - Modelo instanciado.")

        print("#4 - Iniciando monitoramento automático...")
        start_monitoring_system(app, face_model)
        print("#4 - Monitoramento iniciado.")

        print("#5 - Iniciando processamento de incidencia")
        clipService = ClipService(app.state.video_index)
        incident_processor = ProcessingIncident(clipService)
        await incident_processor.start()
        print("#5 - Processamento de incidencia iniciado.")

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
