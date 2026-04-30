from fileinput import filename
import os
import time
import cv2
from cv2.gapi import crop
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process


class FaceWorker(Process):
    """
    Process único compartilhado entre todas as câmeras.

    Carrega o InsightFace uma única vez (~250MB), recebe crops de pessoas
    vindos dos YoloWorkers, extrai embeddings e coloca na result_queue
    para o EmbeddingProcessor de cada câmera.

    Não tem estado por câmera — só extrai embeddings.
    """

    def __init__(self, face_queue, result_queue):
        super().__init__(daemon=True)
        self.face_queue = face_queue
        self.result_queue = result_queue

    def run(self):
        from services.face_recognition_service import FaceModel
        face_model = FaceModel()

        print("✅ FaceWorker pronto")

        while True:
            try:
                item = self.face_queue.get(timeout=1)
                print(f"[FaceWorker] recebi item da fila")
            except:
                print(f"[FaceWorker] fila vazia (timeout)")
                continue

            if item is None:
                break

            camera_id, track_id, crop, face_roi = item

            import cv2
            cv2.imwrite(f"debug_faces/cam{camera_id}_track{track_id}_{int(time.time()*1000)}.jpg", crop)


            try:
                faces = face_model.get_faces(crop)
                # filename = f"debug_faces/cam{camera_id}_track{track_id}_{int(time.time()*1000)}.jpg"
                # cv2.imwrite(filename, crop)
                # print('FACE RCONHECIDA E SALVA EM', filename)
            except Exception as e:
                print(f"[FaceWorker] Erro ao extrair face: {e}")
                continue

            if not faces:
                continue


            # FUNCIONALIDADE PARA FILTRAR AS IMAGENS DO ROSTO PELO TAMANHO DO ROI DEFINIDO PELO USUÁRIO (RoiType=1)
            # print("TAMANHO ROI")
            # print(face_roi["min_width"], face_roi["min_height"])

            # for face in faces:
            #     print("TAMANHO FACE")
            #     print(face.bbox[2] - face.bbox[0], face.bbox[3] - face.bbox[1])


            # if face_roi:
            #     faces = [
            #         f for f in faces
            #         if (f.bbox[2] - f.bbox[0]) >= face_roi["min_width"]
            #         and (f.bbox[3] - f.bbox[1]) >= face_roi["min_height"]
            #     ]

            
            if not faces:  # ← faltou isso
                print(f"[FaceWorker] cam {camera_id} track {track_id} — face pequena demais, ignorada")
                continue
                        

            print(f"👤 FaceWorker extraiu {len(faces)} faces da câmera {camera_id} track {track_id}")

            embeddings = [
                {
                    "embedding": face.normed_embedding.tolist(),
                    "bbox": face.bbox.tolist(),
                }
                for face in faces
            ]

            print(f"👤 FaceWorker extraiu embeddings da câmera {camera_id} track {track_id}: {len(embeddings)}")

            print(f"👤 FaceWorker: result_queue tem {self.result_queue.qsize()} itens")
            if not self.result_queue.full():
                self.result_queue.put_nowait((camera_id, track_id, embeddings))
            else:
                print(f"[⚠] result_queue cheia — embedding câmera {camera_id} track {track_id} descartado")