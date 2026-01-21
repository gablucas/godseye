import numpy as np

class FaceMatcher:
    def __init__(self, ids, emb_matrix):
        """
        ids: List[int]
        emb_matrix: np.ndarray shape (N, D)
        """
        self.ids = ids
        self.emb_matrix = emb_matrix

    def match(self, detected_embedding, threshold=0.65):
        """
        Cosine similarity vetorizada
        """
        # garante numpy
        emb = np.asarray(detected_embedding, dtype=np.float32)

        # produto vetorial (libera GIL, roda em C)
        sims = self.emb_matrix @ emb   # shape (N,)

        best_idx = int(np.argmax(sims))
        best_sim = float(sims[best_idx])

        if best_sim >= threshold:
            return self.ids[best_idx], best_sim

        return None, best_sim
    
    def similarity(self, emb1, emb2):
        """
        Retorna cosine similarity entre dois embeddings normalizados
        """
        return float(np.dot(emb1, emb2))