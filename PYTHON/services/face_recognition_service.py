import insightface

import onnxruntime as ort
print(ort.get_available_providers())

class FaceModel:
    def __init__(self):
        self.app = insightface.app.FaceAnalysis(
            name="antelopev2",
            root="C:/Users/Gab/Documents/Fabrica/GodsEye/PYTHON",
            providers=[
                "CUDAExecutionProvider",
                "CPUExecutionProvider"
            ]
        )
        self.app.prepare(ctx_id=0)

    def get_faces(self, img):
        return self.app.get(img)

    def get_embedding(self, img):
        faces = self.app.get(img)
        if not faces:
            return None
        return faces[0].normed_embedding

