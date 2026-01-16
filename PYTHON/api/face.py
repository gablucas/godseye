from fastapi import APIRouter, UploadFile, File, HTTPException, Depends
from fastapi.responses import JSONResponse
from dependencies import get_face_model

from services.face_recognition_service import FaceModel
from services.image_processing_service import read_image

router = APIRouter()

@router.post("/face/embedding")
async def face_embedding(photo: UploadFile = File(...), face_model: FaceModel = Depends(get_face_model)):
    bytes_data = await photo.read()
    img = read_image(bytes_data)

    if img is None:
        raise HTTPException(status_code=400, detail="Imagem inválida")

    embedding = face_model.get_embedding(img)

    if embedding is None:
        raise HTTPException(status_code=404, detail="Nenhum rosto encontrado")

    return JSONResponse(
        content={"embedding": embedding.tolist()}
    )
