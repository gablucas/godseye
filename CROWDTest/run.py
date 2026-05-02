from datetime import datetime
import random
from zoneinfo import ZoneInfo
import time

from app.extracted_embedding import ExtractedEmbedding
from app.data import persons, cameras
from app.send_extracted_embedding import SendExtractedEmbedding

### ULTRAPASSA O TEMPO MAXIMO

embedding_sender = SendExtractedEmbedding()

# payload = ExtractedEmbedding (
#     CameraId=45,
#     Embedding=persons[0]["embedding"],
#     IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#     )


# embedding_sender.send_extracted_embedding(payload)

# time.sleep(5)

# ### NÃO FICA O TEMPO MÍNIMO

# payload = ExtractedEmbedding (
#     CameraId=45,
#     Embedding=persons[1]["embedding"],
#     IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#     )

# embedding_sender.send_extracted_embedding(payload)

# time.sleep(10)

# payload = ExtractedEmbedding (
#     CameraId=45,
#     Embedding=persons[1]["embedding"],
#     IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#     )

# embedding_sender.send_extracted_embedding(payload)

# time.sleep(5)

# ### NÃO VAI PRO OUTRO SETOR

# payload = ExtractedEmbedding (
#     CameraId=45,
#     Embedding=persons[2]["embedding"],
#     IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#     )

# embedding_sender.send_extracted_embedding(payload)

# time.sleep(80)

# payload = ExtractedEmbedding (
#     CameraId=45,
#     Embedding=persons[2]["embedding"],
#     IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#     )

# embedding_sender.send_extracted_embedding(payload)

# time.sleep(5)

# print("Testes concluídos.")

while True:
    SLEEP_TIME = random.uniform(0.5, 2.0)  
    CROWD_SIZE = random.randint(1, len(persons) - 1)
    START_PERSON_INDEX = random.randint(0, len(persons) - CROWD_SIZE)
    CAMERA_INDEX = random.randint(0, len(cameras) - 1)

    for person in persons[START_PERSON_INDEX:START_PERSON_INDEX + CROWD_SIZE]:
        payload = ExtractedEmbedding (
            CameraId=cameras[CAMERA_INDEX]["id"],
            Embedding=person["embedding"],
            IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
        )

        embedding_sender.send_extracted_embedding(payload)

        print(f' [x] Sent embedding for person {person["id"]} from camera {cameras[CAMERA_INDEX]["id"]}')

    time.sleep(SLEEP_TIME)


# for camera in cameras[0:1]:
#     print(camera)




# for person in persons:
#     for camera in cameras:

#         payload = ExtractedEmbedding (
#             CameraId=camera["id"],
#             Embedding=person["embedding"],
#             IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
#         )

#         embedding_sender.send_extracted_embedding(payload)

#         print(f' [x] Sent embedding for person {person["id"]} from camera {camera["id"]}')
        # time.sleep(.2)  # Simula um intervalo entre os envios