import os
import subprocess
import tempfile
import time
from datetime import datetime, timedelta

from core.video_index import VideoIndex

PRE_EVENT_SECONDS = 5
POST_EVENT_SECONDS = 10
MAX_WAIT_SECONDS = 60  # quanto tempo esperar o pós-evento existir


class ClipService:

    def __init__(self, video_index: VideoIndex):
        self.video_index = video_index

    def generate_event_clip(self, camera_id: str, event_time: datetime) -> str:
        print("INDEX DO VIDEO")
        print(self.video_index)


        # Tenta buscar o segmento o mais rápido possível
        print("HORARIO DO EVENTO")
        print(event_time)

        clip_start = event_time - timedelta(seconds=PRE_EVENT_SECONDS)
        print("CLIP START")
        print(clip_start)

        duration = PRE_EVENT_SECONDS + POST_EVENT_SECONDS
        print("DURATION")
        print(duration)

        print("PEGAR SEGMENTOS...")
        # Tente encontrar o segmento
        segments = self._get_segments(str(camera_id), clip_start, duration)

        os.makedirs("clips", exist_ok=True)

        output_path = (
            f"clips/{camera_id}_"
            f"{event_time.strftime('%Y%m%d_%H%M%S')}.mkv"
        )

        # Arquivo para concatenação
        with tempfile.NamedTemporaryFile(mode="w", delete=False, suffix=".txt") as f:
            list_file = f.name
            for seg in segments:
                f.write(f"file '{os.path.abspath(seg['path'])}'\n")

        offset = (clip_start - segments[0]["start"]).total_seconds()

        # Comando ffmpeg para gerar o clip
        cmd = [
            "ffmpeg",
            "-y",
            "-ss", str(offset),
            "-f", "concat",
            "-safe", "0",
            "-i", list_file,
            "-t", str(duration),
            "-c:v", "libx264",
            "-preset", "veryfast",
            "-crf", "23",
            "-c:a", "aac",
            "-movflags", "+faststart",
            output_path
        ]

        subprocess.run(cmd, check=True)

        return output_path

    def _get_segments(self, camera_id: str, clip_start: datetime, duration: int):
        event_time = clip_start + timedelta(seconds=PRE_EVENT_SECONDS)
        required_time = event_time + timedelta(seconds=120)

        print("\n========== VIDEO INDEX ANTES ==========")
        for cam_id, segments in self.video_index.index.items():
            print(f"\n📷 CÂMERA: {cam_id}")
            print(f"   Total de segmentos: {len(segments)}")

            for seg in segments[-5:]:  # mostra só os últimos 5
                print(
                    f"   ▶ {seg['start']} -> {seg['end']} | {seg['path']}"
                )
        print("=================================\n")
        
        # ⏳ Espera o tempo REAL passar
        while datetime.now() < required_time:
            time.sleep(0.5)


        self.video_index.update()

        print("\n========== VIDEO INDEX DEPOIS ==========")
        for cam_id, segments in self.video_index.index.items():
            print(f"\n📷 CÂMERA: {cam_id}")
            print(f"   Total de segmentos: {len(segments)}")

            for seg in segments[-5:]:  # mostra só os últimos 5
                print(
                    f"   ▶ {seg['start']} -> {seg['end']} | {seg['path']}"
                )
        print("=================================\n")

        waited = 0
        while waited < MAX_WAIT_SECONDS:
              # 🔁 agora sim faz sentido
            try:
                return self.video_index.find_segments(
                    camera_id=camera_id,
                    start_dt=clip_start,
                    duration=duration
                )
            except Exception:
                time.sleep(1)
                waited += 1

        raise Exception("Segmentos do pós-evento ainda não disponíveis")
