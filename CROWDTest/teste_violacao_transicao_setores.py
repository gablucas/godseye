from datetime import datetime
import random
from zoneinfo import ZoneInfo
import time

from app.extracted_embedding import ExtractedEmbedding
from app.data import persons, cameras
from app.send_extracted_embedding import SendExtractedEmbedding

### ULTRAPASSA O TEMPO MAXIMO

embedding_sender = SendExtractedEmbedding()

payload = ExtractedEmbedding (
    CameraId=45,
    Embedding=persons[0]["embedding"],
    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )


embedding_sender.send_extracted_embedding(payload)

time.sleep(5)

### NÃO FICA O TEMPO MÍNIMO

payload = ExtractedEmbedding (
    CameraId=45,
    Embedding=persons[1]["embedding"],
    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )

embedding_sender.send_extracted_embedding(payload)

time.sleep(10)

payload = ExtractedEmbedding (
    CameraId=45,
    Embedding=persons[1]["embedding"],
    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )

embedding_sender.send_extracted_embedding(payload)

time.sleep(5)

### NÃO VAI PRO OUTRO SETOR

payload = ExtractedEmbedding (
    CameraId=45,
    Embedding=persons[2]["embedding"],
    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )

embedding_sender.send_extracted_embedding(payload)

time.sleep(80)

payload = ExtractedEmbedding (
    CameraId=45,
    Embedding=persons[2]["embedding"],
    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )

embedding_sender.send_extracted_embedding(payload)

time.sleep(5)

print("Testes concluídos.")