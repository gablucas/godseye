from services.recorder_service import CameraRecorder

_recorders = {}

def start_recorder(camera_id, rtsp_url):
    if camera_id in _recorders:
        return

    recorder = CameraRecorder(
        camera_id=camera_id,
        rtsp_url=rtsp_url,
        output_dir=f"records/{camera_id}"
    )
    recorder.start()
    _recorders[camera_id] = recorder


def stop_all_recorders():
    for recorder in _recorders.values():
        recorder.stop()
    _recorders.clear()
