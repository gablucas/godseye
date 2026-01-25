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

from core.godseyedata import GodsEyeData
from core.godseyedata_loader import load_godseye_data_from_api
from core.video_index import VideoIndex
from core.incident_processing import ProcessingIncident
from application.monitor_manager import MonitorManager

from domain.data_validation import MonitoringDataError
from infrastructure.log_queue import start_log_worker
from services.clip_service import ClipService
from services.face_matcher_service import FaceMatcher
from services.face_processor_service import FaceRecognitionProcessor
from services.monitoring_service import validate_monitoring_data
from dependencies import get_face_model

from contextlib import asynccontextmanager
from fastapi import FastAPI
from core.startup_retry import load_godseye_with_retry

# LIFESPAN
@asynccontextmanager
async def lifespan(app: FastAPI):
    app.state.godseye = GodsEyeData()
    app.state.video_index = VideoIndex()
    app.state.background_started = False

    start_log_worker()

    asyncio.create_task(load_godseye_with_retry(app))
    yield

app = FastAPI(lifespan=lifespan)

# @asynccontextmanager
# async def lifespan(app: FastAPI):
#     # ===== STARTUP =====
#     try:
#         app.state.godseye = GodsEyeData()
#         app.state.video_index = VideoIndex()
#         app.state.background_started = False

#         start_log_worker()

#         print("#1 - Buscando dados do GodsEye...")
#         data = await load_godseye_data_from_api()

#         print("#2 - Validando dados...")
#         cameras, persons = validate_monitoring_data(data)

#         print("#3 - Construindo matcher...")
#         ids = [p["Id"] for p in persons]
#         embeddings = [p["Embedding"] for p in persons]

#         emb_matrix = np.asarray(embeddings, dtype=np.float32)
#         face_matcher = FaceMatcher(ids=ids, emb_matrix=emb_matrix)

#         print("#4 - Instanciando modelo facial...")
#         face_model = get_face_model()

#         print("#5 - Criando FaceRecognitionProcessor...")
#         face_processor = FaceRecognitionProcessor(
#             face_model=face_model,
#             face_matcher=face_matcher
#         )

#         # guarda no estado global
#         app.state.face_model = face_model
#         app.state.face_matcher = face_matcher
#         app.state.face_processor = face_processor

#         print("#4 - Vinculado dados ao estado global...")
#         app.state.godseye.set(data)

#         print("#5 - Construindo índice de vídeos...")
#         app.state.video_index.build()

#         print("#5 - Iniciando monitoramento automático...")
#         monitor_manager = MonitorManager(face_model, face_matcher, cameras)

#         monitor_manager.start_monitoring()

#         print("#6 - Iniciando processamento de incidencia")
#         clip_service = ClipService(app.state.video_index)

#         incident_processor = ProcessingIncident(
#             clip_service=clip_service,
#             face_model=face_model,
#             face_matcher=face_matcher
#         )

#         asyncio.create_task(incident_processor.start())

#     except MonitoringDataError as e:
#         print("❌ Erro de validação:", e)
#         raise RuntimeError("Falha ao iniciar monitoramento")

#     except Exception as e:
#         print("🔥 Erro crítico no startup:", e)
#         raise

#     yield

#     print("Monitoramento encerrado")


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
app.include_router(video_router)
app.include_router(face_router, prefix="/api")
app.include_router(monitor_router, prefix="/api")
app.include_router(stream_router, prefix="/api")
app.include_router(clip_router, prefix="/api")
