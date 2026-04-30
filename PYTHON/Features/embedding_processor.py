import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Event
from datetime import datetime
from zoneinfo import ZoneInfo
import numpy as np
import time

from infrastructure.send_extracted_embedding import SendExtractedEmbedding
from schemas.extracted_embedding import ExtractedEmbedding


class EmbeddingProcessor(Process):
    """
    Process único global (não mais por câmera).

    - Drena a result_queue por até BATCH_WINDOW segundos
    - Deduplica por câmera via similaridade de cosseno
    - Envia todos os embeddings novos em um único lote para o Rabbit
    
    Elimina a race condition do modelo anterior (N processos
    competindo pela mesma fila e devolvendo itens).
    """

    SIMILARITY_THRESHOLD = 0.6
    BATCH_WINDOW = 1.0  # segundos aguardando antes de enviar o lote

    def __init__(self, result_queue):
        super().__init__(daemon=True)
        self.result_queue = result_queue
        self._stop_event = Event()

    def stop(self):
        self._stop_event.set()

    def run(self):
        self.embedding_sender = SendExtractedEmbedding()

        # Histórico de embeddings já enviados, separado por câmera
        # { camera_id -> [np.array, ...] }
        self.sent_embeddings: dict[str, list[np.ndarray]] = {}

        while not self._stop_event.is_set():
            batch = self._collect_batch()

            if not batch:
                continue

            new_embeddings = self._filter_new(batch)

            if not new_embeddings:
                continue

            print(f"📤 EmbeddingProcessor — enviando lote com {len(new_embeddings)} embedding(s)")
            self._send_batch(new_embeddings)

    def _collect_batch(self) -> list[tuple]:
        """
        Drena a fila durante BATCH_WINDOW segundos.
        Retorna todos os itens coletados no período.
        """
        deadline = time.monotonic() + self.BATCH_WINDOW
        items = []

        while time.monotonic() < deadline and not self._stop_event.is_set():
            try:
                item = self.result_queue.get(timeout=0.05)
                items.append(item)
            except:
                pass  # fila vazia, continua aguardando até o deadline

        return items

    def _filter_new(self, batch: list[tuple]) -> list[ExtractedEmbedding]:
        """
        Para cada item do lote:
          - pega todos os embeddings (um por face detectada na crop)
          - filtra os que já foram enviados para aquela câmera (cosseno)
          - registra os novos no histórico

        Não escolhe o "melhor" — processa cada face individualmente,
        pois um item pode conter mais de uma pessoa diferente.
        """
        new_embeddings = []
        now = datetime.now(ZoneInfo("America/Sao_Paulo"))

        for (camera_id, track_id, embeddings) in batch:
            if not embeddings:
                continue

            cam_history = self.sent_embeddings.setdefault(camera_id, [])

            for face in embeddings:
                emb = np.array(face["embedding"])

                if not self._is_new_person(emb, cam_history):
                    continue

                cam_history.append(emb)

                new_embeddings.append(
                    ExtractedEmbedding(
                        CameraId=camera_id,
                        Embedding=emb.tolist(),
                        IdentifiedAt=now,
                    )
                )

        return new_embeddings

    def _is_new_person(self, embedding: np.ndarray, history: list[np.ndarray]) -> bool:
        """Similaridade de cosseno. Embeddings assumidos como já normalizados."""
        for sent in history:
            if float(np.dot(embedding, sent)) > self.SIMILARITY_THRESHOLD:
                return False
        return True

    def _send_batch(self, embeddings: list[ExtractedEmbedding]):
        """
        Envia a lista inteira em uma única chamada.
        Se o Rabbit não aceitar lista nativamente, serializa como JSON array.
        """
        try:
            self.embedding_sender.send_batch(embeddings)
        except Exception as e:
            print(f"[EmbeddingProcessor] Erro ao enviar lote: {e}")