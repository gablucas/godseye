from datetime import datetime
import random
from zoneinfo import ZoneInfo
import time

from app.extracted_embedding import ExtractedEmbedding
from app.data import persons, cameras
from app.send_extracted_embedding import SendExtractedEmbedding

### ULTRAPASSA O TEMPO MAXIMO

embedding_sender = SendExtractedEmbedding()

for person in persons:
    SLEEP_TIME = random.uniform(1, 5)
    CAMERA_INDEX = random.randint(0, len(cameras) - 1)

    payload = ExtractedEmbedding(
        CameraId=45,
        Embedding=person["embedding"],
        IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
    )

    embedding_sender.send_extracted_embedding(payload)

    if(random.randint(0, 1) == 0):
        time.sleep(SLEEP_TIME * 2)

        payload = ExtractedEmbedding(
            CameraId=45,
            Embedding=person["embedding"],
            IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
        )

        embedding_sender.send_extracted_embedding(payload)

        if(random.randint(0, 1) == 0):
            time.sleep(SLEEP_TIME)
            
            payload = ExtractedEmbedding(
                CameraId=51,
                Embedding=person["embedding"],
                IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
            )

            embedding_sender.send_extracted_embedding(payload)

            if(random.randint(0, 4) == 0):
                time.sleep(SLEEP_TIME)
                
                payload = ExtractedEmbedding(
                    CameraId=51,
                    Embedding=person["embedding"],
                    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
                )

                embedding_sender.send_extracted_embedding(payload)



    print(f' [x] Sent embedding for person {person["id"]} from camera {cameras[CAMERA_INDEX]["id"]}')

    time.sleep(SLEEP_TIME)  