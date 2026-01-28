def has_feature(camera, feature_id: int) -> bool:
    features = camera.get("Features") or []
    return any(f.get("Id") == feature_id for f in features)