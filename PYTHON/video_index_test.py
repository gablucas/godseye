from core.video_index import VideoIndex
video_index = VideoIndex()
video_index.build()

print("\n========== VIDEO INDEX ANTES ==========")
for cam_id, segments in video_index.index.items():
    print(f"\n📷 CÂMERA: {cam_id}")
    print(f"   Total de segmentos: {len(segments)}")

    for seg in segments[-5:]:  # mostra só os últimos 5
        print(
            f"   ▶ {seg['start']} -> {seg['end']} | {seg['path']}"
        )
print("=================================\n")