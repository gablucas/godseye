import subprocess
import os
import time

# =========================
# CONFIGURAÇÕES GERAIS
# =========================
RTSP_HOST = "rtsp://localhost:8554"
VIDEO_DIR = "C:\\Users\\Gab\\Pictures\\FINAL\\Test"

# RESOLUTION = "1280x720"   # 1920x1080 ou 1280x720
FPS = 15                   # FPS típico de câmera IP
BITRATE = "2500k"          # 1500k (720p) | 2500k (1080p)
GOP_SECONDS = 2             # keyframe a cada X segundos

# =========================
# CÁLCULOS AUTOMÁTICOS
# =========================
GOP = FPS * GOP_SECONDS
BUF_SIZE = str(int(BITRATE.replace("k", "")) * 2) + "k"

# =========================
# FFmpeg BASE
# =========================
FFMPEG_CMD_BASE = [
    "ffmpeg",
    "-re",
    "-stream_loop", "-1",
]

# =========================
# INICIAR STREAMS
# =========================
processes = []

for idx, video in enumerate(os.listdir(VIDEO_DIR), start=1):
    if not video.lower().endswith((".mp4", ".avi", ".mkv")):
        continue

    stream_name = f"camera{idx}"
    video_path = os.path.join(VIDEO_DIR, video)

    # VER. 1
    # cmd = FFMPEG_CMD_BASE + [
    #     "-i", video_path,
    #     "-c:v", "libx264",
    #     "-profile:v", "baseline",
    #     "-preset", "veryfast",
    #     "-tune", "zerolatency",
    #     "-pix_fmt", "yuv420p",
    #     "-s", RESOLUTION,
    #     "-r", str(FPS),
    #     "-g", str(GOP),
    #     "-keyint_min", str(GOP),
    #     "-b:v", BITRATE,
    #     "-maxrate", BITRATE,
    #     "-bufsize", BUF_SIZE,
    #     "-an",
    #     "-f", "rtsp",
    #     f"{RTSP_HOST}/{stream_name}"
    # ]

    cmd = FFMPEG_CMD_BASE + [
        "-i", video_path,

        # === ENCODER ===
        "-c:v", "libx264",
        "-profile:v", "main",          # mais robusto que baseline
        "-preset", "fast",             # menos perda que veryfast
        "-tune", "zerolatency",

        # === FORMATO ===
        "-pix_fmt", "yuv420p",
        # "-s", RESOLUTION,
        "-r", str(FPS),

        # === GOP / KEYFRAMES ===
        "-g", str(GOP),
        "-keyint_min", str(GOP),
        "-sc_threshold", "0",           # evita keyframes aleatórios

        # === BITRATE ===
        "-b:v", BITRATE,
        "-maxrate", BITRATE,
        "-bufsize", "8000k",            # buffer maior → menos corrupção

        # === RTSP ===
        "-an",
        "-f", "rtsp",
        "-rtsp_transport", "tcp",       # MUITO importante para estabilidade
        f"{RTSP_HOST}/{stream_name}"
    ]

    print(f"[INFO] Iniciando {stream_name} -> {video}")
    process = subprocess.Popen(cmd)
    processes.append(process)
    time.sleep(0.5)  # evita pico de CPU ao iniciar vários

print("\n[OK] Streams RTSP em execução")
print("Pressione CTRL+C para encerrar")

try:
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    print("\n[INFO] Encerrando streams...")
    for p in processes:
        p.terminate()
