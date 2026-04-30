def validate_monitoring_data(monitoring_data: dict):
    cameras = monitoring_data.get("Cameras", [])

    if not cameras:
        raise ValueError("Nenhuma câmera encontrada")

    return cameras