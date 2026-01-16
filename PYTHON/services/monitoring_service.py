def validate_monitoring_data(monitoring_data: dict):
    cameras = monitoring_data.get("Cameras", [])
    persons = monitoring_data.get("Persons", [])

    if not cameras:
        raise ValueError("Nenhuma câmera encontrada")

    if not persons:
        raise ValueError("Nenhuma pessoa encontrada")

    return cameras, persons