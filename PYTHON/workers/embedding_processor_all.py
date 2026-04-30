import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Event
from datetime import datetime
from zoneinfo import ZoneInfo
import numpy as np

from infrastructure.send_extracted_embedding import SendExtractedEmbedding
from schemas.extracted_embedding import ExtractedEmbedding

class EmbeddingProcessor(Process):
    """
    Processo Global de Extração.
    Consome frames de todas as câmeras e processa TODAS as faces 
    detectadas em cada frame.
    """

    SIMILARITY_THRESHOLD = 0.6

    def __init__(self, result_queue):
        super().__init__(daemon=True)
        self.result_queue = result_queue
        self._stop_event = Event()

    def stop(self):
        self._stop_event.set()

    def run(self):
        self.embedding_sender = SendExtractedEmbedding()
        
        # {camera_id: [lista_de_embeddings_enviados]}
        self.sent_embeddings_by_camera = {}
        
        # self.face_extractor = FaceExtractor()
        print("🚀 Processador Global iniciado (Processando múltiplas pessoas por frame)")

        while not self._stop_event.is_set():
            try:
                camera_id, frame = self.result_queue.get(timeout=1.0)
            except:
                continue

            # 1. Extração de TODAS as faces do frame
            # faces deve ser uma lista de dicionários
            faces = [] # Substitua por: self.face_extractor.detect_all(frame)

            if not faces:
                continue

            # 2. Iterar por cada face encontrada na imagem
            for face_data in faces:
                embedding = np.array(face_data["embedding"])

                # 3. Verificar se ESTA face específica é nova para ESTA câmera
                if not self._is_new_person(camera_id, embedding):
                    continue

                # 4. Se for nova, registra e envia
                if camera_id not in self.sent_embeddings_by_camera:
                    self.sent_embeddings_by_camera[camera_id] = []
                
                # Adiciona ao cache de "já enviados"
                self.sent_embeddings_by_camera[camera_id].append(embedding)

                # Limita o cache por câmera para não estourar a RAM (ex: últimos 200)
                if len(self.sent_embeddings_by_camera[camera_id]) > 200:
                    self.sent_embeddings_by_camera[camera_id].pop(0)
                
                payload = ExtractedEmbedding(
                    CameraId=camera_id,
                    Embedding=embedding.tolist(),
                    IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo")),
                )
                
                try:
                    self.embedding_sender.send_extracted_embedding(payload)
                    print(f"📤 Face enviada! Câmera: {camera_id} (Total na imagem: {len(faces)})")
                except Exception as e:
                    print(f"❌ Erro ao enviar: {e}")

    def _is_new_person(self, camera_id, embedding):
        """
        Compara o embedding atual com o histórico daquela câmera.
        """
        sent_list = self.sent_embeddings_by_camera.get(camera_id, [])
        
        for sent in sent_list:
            # Similaridade de Cosseno via Produto Escalar
            similarity = float(np.dot(embedding, sent))
            if similarity > self.SIMILARITY_THRESHOLD:
                return False
        return True