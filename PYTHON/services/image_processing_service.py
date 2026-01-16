import numpy as np
import cv2

def read_image(file_bytes: bytes) -> np.ndarray | None:
    np_arr = np.frombuffer(file_bytes, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
    return img