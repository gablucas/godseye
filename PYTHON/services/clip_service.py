import os
import subprocess
import tempfile
import time
from datetime import datetime, timedelta

from core.video_index import VideoIndex

PRE_EVENT_SECONDS = 2
POST_EVENT_SECONDS = 2
MAX_WAIT_SECONDS = 20  # quanto tempo esperar o pós-evento existir


class ClipService:

    def __init__(self, video_index: VideoIndex):
        self.video_index = video_index

    def generate_event_clip(self, camera_id: str, event_time: datetime) -> tuple[str, str]:
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
        print("SEGMENTOS ENCONTRADOS:")
        print(segments)

        os.makedirs("clips", exist_ok=True)

        # MKV
        # file_name = f"{camera_id}_{event_time.strftime('%Y%m%d_%H%M%S')}.mkv"
        # output_path = f"clips/{file_name}"

        # MP4
        file_name = f"{camera_id}_{event_time.strftime('%Y%m%d_%H%M%S')}.mp4"
        output_path = f"clips/{file_name}"

        # Arquivo para concatenação
        with tempfile.NamedTemporaryFile(mode="w", delete=False, suffix=".txt") as f:
            list_file = f.name
            for seg in segments:
                f.write(f"file '{os.path.abspath(seg['path'])}'\n")

        offset = (clip_start - segments[0]["start"]).total_seconds()


        # Comando ffmpeg para gerar o clip - MKV
        # cmd = [
        #     "ffmpeg",
        #     "-y",
        #     "-ss", str(offset),
        #     "-f", "concat",
        #     "-safe", "0",
        #     "-i", list_file,
        #     "-t", str(duration),
        #     "-c:v", "libx264",
        #     "-preset", "veryfast",
        #     "-crf", "23",
        #     "-c:a", "aac",
        #     "-movflags", "+faststart",
        #     output_path
        # ]

        # Comando ffmpeg para gerar o clip - MP4
        cmd = [
            "ffmpeg",
            "-y",

            # 🔑 SEEK PRECISO
            "-ss", str(offset),

            # 🔗 CONCATENAÇÃO
            "-f", "concat",
            "-safe", "0",
            "-i", list_file,

            # ⏱️ DURAÇÃO FINAL
            "-t", str(duration),

            # 🎥 VÍDEO WEB SAFE
            "-c:v", "libx264",
            "-profile:v", "main",
            "-level", "4.0",
            "-pix_fmt", "yuv420p",
            "-preset", "veryfast",
            "-crf", "23",

            # 🔊 ÁUDIO
            "-c:a", "aac",
            "-b:a", "128k",

            # 🌐 STREAMING
            "-movflags", "+faststart",

            output_path
        ]

        try:
            subprocess.run(cmd, check=True)
        except subprocess.CalledProcessError as e:
            raise RuntimeError("Erro ao gerar clip") from e

        return file_name, output_path

    def _get_segments(self, camera_id: str, clip_start: datetime, duration: int):
        timeout_seconds = 120
        check_interval = 5

        deadline = datetime.now() + timedelta(seconds=timeout_seconds)

        print("⏳ Procurando segmentos...")
        print("📌 Clip start:", clip_start)
        print("🕒 Agora:", datetime.now())
        print("⏱️ Timeout:", timeout_seconds, "s")

        while datetime.now() < deadline:
            self.video_index.update()

            try:
                return self.video_index.find_segments(
                    camera_id=camera_id,
                    start_dt=clip_start,
                    duration=duration
                )
            except Exception:
                time.sleep(check_interval)

        raise Exception(
            f"Segmentos não encontrados após {timeout_seconds}s "
            f"(evento pode ser antigo, mas arquivos não estão disponíveis)"
        )