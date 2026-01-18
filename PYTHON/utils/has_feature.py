def has_feature(camera, feature_id: int) -> bool:
    return any(f["Id"] == feature_id for f in camera.get("Features", []))