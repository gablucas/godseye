import os
os.environ["NO_ALBUMENTATIONS_UPDATE"] = "1"

from multiprocessing import Process, Queue, Event
from threading import Thread
from datetime import datetime
from zoneinfo import ZoneInfo
import cv2
import numpy as np
import time

import PyNvVideoCodec as nvc
import torch

from infrastructure.send_extracted_embedding import SendExtractedEmbedding
from schemas.extracted_embedding import ExtractedEmbedding
from infrastructure.ffmpeg_capture import ffmpeg_capture


class CameraProcess(Process):
    def __init__(self, camera_id, rtsp_url, sector_id, roi, features=None,
                 log_queue=None, shared_person=None,
                 inference_queue=None, result_queue=None):
        super().__init__(daemon=True)
        self.camera_id = camera_id
        self.rtsp_url = rtsp_url
        self.sector_id = sector_id
        self.roi = roi
        self.features = features or {}
        self.stop_event = Event()
        self.log_queue = log_queue
        self.shared_person = shared_person or {}
        self.inference_queue = inference_queue
        self.result_queue = result_queue
        self.width = 640
        self.height = 360

    def stop(self):
        self.stop_event.set()

    def run(self):
        self.embedding_sender = SendExtractedEmbedding()
        self.processed_tracks = set()
        self.active_tracks = {}
        self.target_fps = 1.0
        self.last_run = 0.0
        self.last_detection = 0.0
        self.bg = cv2.createBackgroundSubtractorMOG2(
            history=500, varThreshold=16, detectShadows=False
        )

        capture_thread = Thread(target=self.capture_loop)
        capture_thread.start()

        # Loop principal: processa resultados que vieram do InferenceWorker
        while not self.stop_event.is_set():
            self.process_results()
            time.sleep(0.01)

    def capture_loop(self):
        # Inicia gravação separada se necessário (mantém seu ffmpeg só pra isso)
        if self.features.get("record_path"):
            Thread(target=lambda: ffmpeg_capture(
                rtsp_url=self.rtsp_url,
                width=self.width, height=self.height,
                cameraId=self.camera_id,
                features=self.features,
                record_path=f"records/{self.camera_id}"
            ), daemon=True).start()

        if not (self.features.get("environment_monitoring") or
                self.features.get("dwell_time_monitoring")):
            return

        try:
            demuxer = nvc.PyFFmpegDemuxer(self.rtsp_url)
            decoder = nvc.PyNvDecoder(
                self.width, self.height,
                demuxer.Format(),
                nvc.CudaVideoCodec.H264,
                gpu_id=0
            )
        except Exception as e:
            print(f"❌ PyNvVideoCodec falhou (câmera {self.camera_id}): {e}")
            print("↩️  Fallback para FFmpeg CPU")
            self._capture_loop_cpu_fallback()  # seu método original
            return

        packet = nvc.PacketData()

        try:
            while not self.stop_event.is_set():
                if not demuxer.DemuxSinglePacket(packet):
                    time.sleep(0.1)
                    continue

                frames = decoder.DecodeSinglePacket(packet)
                for surface in frames:
                    now = time.time()

                    # Converte surface NV12 (VRAM) → tensor RGB na GPU
                    frame_tensor = self._surface_to_tensor(surface)

                    # Controle de FPS por movimento
                    # ⚠️ bg subtractor precisa de numpy — faz download só pra isso
                    frame_cpu = frame_tensor.cpu().numpy()
                    self._update_target_fps(frame_cpu, now)

                    frame_interval = 1.0 / self.target_fps
                    if now - self.last_run < frame_interval:
                        continue

                    print(f"🎥 Câmera {self.camera_id} — processando frame (GPU, FPS: {self.target_fps:.1f})")
                    self.last_run = now

                    if not self.inference_queue.full():
                        # Envia tensor GPU direto — zero-copy na inferência
                        self.inference_queue.put_nowait((self.camera_id, frame_tensor))

        finally:
            pass  # PyNvVideoCodec não precisa de kill

    def _surface_to_tensor(self, surface) -> "torch.Tensor":
        """Converte NV12 surface da VRAM para tensor RGB (ainda na GPU)."""
        import torch
        import PyNvVideoCodec as nvc

        converter = nvc.PySurfaceConverter(
            self.width, self.height,
            nvc.PixelFormat.NV12,
            nvc.PixelFormat.RGB,
            gpu_id=0
        )
        rgb_surface = converter.Execute(surface, nvc.ColorspaceConversionContext(
            nvc.ColorSpace.BT_601, nvc.ColorRange.MPEG
        ))

        downloader = nvc.PySurfaceDownloader(
            self.width, self.height, nvc.PixelFormat.RGB, gpu_id=0
        )
        # Mantém na GPU como tensor torch
        frame_tensor = torch.zeros(
            (self.height, self.width, 3), dtype=torch.uint8, device="cuda:0"
        )
        downloader.DownloadSingleSurface(rgb_surface, frame_tensor)
        return frame_tensor


    def _update_target_fps(self, frame, now):
        mask = self.bg.apply(frame)
        motion_pixels = cv2.countNonZero(mask)

        if motion_pixels > 5000:
            self.last_detection = now
            self.target_fps = 10.0
        elif len(self.active_tracks) > 0:
            self.last_detection = now
            self.target_fps = 5.0
        elif motion_pixels > 500 or (now - self.last_detection < 2.0):
            self.target_fps = 5.0
        else:
            self.target_fps = 1.0

    def process_results(self):
        # Coleta todos os resultados desta câmera de uma vez
        pending = []
        requeue = []

        while not self.result_queue.empty():
            try:
                item = self.result_queue.get_nowait()
            except:
                break

            cam_id = item[0]
            if cam_id == self.camera_id:
                pending.append(item)
            else:
                requeue.append(item)

        # Devolve os que não são desta câmera
        for item in requeue:
            self.result_queue.put(item)

        if not pending:
            return

        # Pega só o resultado mais recente por track (descarta duplicatas antigas)
        now = time.time()
        latest_by_track = {}
        for (camera_id, tracker_ids, boxes, embeddings_by_track) in pending:
            for i, track_id in enumerate(tracker_ids):
                latest_by_track[track_id] = {
                    "box": boxes[i],
                    "faces": embeddings_by_track.get(track_id)
                }
                self.active_tracks[track_id] = now

        # Processa cada track uma única vez
        for track_id, data in latest_by_track.items():
            if track_id in self.processed_tracks:
                continue

            faces = data["faces"]
            if not faces or len(faces) == 0:
                continue

            emb = self._get_embedding(faces, data["box"])
            
            if emb is None or len(emb) != 512:
                continue

            self.processed_tracks.add(track_id)
            print(f"📤 Câmera {self.camera_id} — enviando embedding track {track_id}")

            payload = ExtractedEmbedding(
                CameraId=self.camera_id,
                Embedding=emb,
                IdentifiedAt=datetime.now(ZoneInfo("America/Sao_Paulo"))
            )
            self.embedding_sender.send_extracted_embedding(payload)

        # Limpeza de tracks perdidos
        # (usa o último batch recebido como referência)
        last_camera_id, last_tracker_ids, _, _ = pending[-1]
        current = set(last_tracker_ids)
        for tid in list(self.active_tracks.keys()):
            if tid not in current:
                self.active_tracks.pop(tid, None)
                self.processed_tracks.discard(tid)

    def _get_embedding(self, faces, box):
        # faces agora é lista de dicts: [{"embedding": [...], "bbox": [...]}]
        face_roi = None
        if self.roi:
            face_roi = next((r for r in self.roi if r["RoiType"] == 1), None)

        if face_roi:
            coords = face_roi["Coordinates"]
            min_w = coords["Width"] * self.width
            min_h = coords["Height"] * self.height

            def face_area(f):
                x1, y1, x2, y2 = f["bbox"]
                return (x2 - x1) * (y2 - y1)

            face = max(faces, key=face_area)
            x1, y1, x2, y2 = face["bbox"]
            if (x2 - x1) < min_w or (y2 - y1) < min_h:
                return None
            return face["embedding"]
        else:
            return faces[0]["embedding"]