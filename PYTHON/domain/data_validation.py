# domain/monitoring_validation.py

class MonitoringDataError(Exception):
    pass


def validate_monitoring_data(monitoring_data: dict):
    cameras = monitoring_data.get("Cameras")
    persons = monitoring_data.get("Persons")

    if not cameras:
        raise MonitoringDataError("Nenhuma câmera encontrada")

    if not persons:
        raise MonitoringDataError("Nenhuma pessoa encontrada")

    return cameras, persons