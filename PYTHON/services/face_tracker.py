import numpy as np
from filterpy.kalman import KalmanFilter
from scipy.optimize import linear_sum_assignment


def iou(bb_test, bb_gt):
    xx1 = np.maximum(bb_test[0], bb_gt[0])
    yy1 = np.maximum(bb_test[1], bb_gt[1])
    xx2 = np.minimum(bb_test[2], bb_gt[2])
    yy2 = np.minimum(bb_test[3], bb_gt[3])
    w = np.maximum(0., xx2 - xx1)
    h = np.maximum(0., yy2 - yy1)
    wh = w * h
    o = wh / (
        (bb_test[2] - bb_test[0]) * (bb_test[3] - bb_test[1]) +
        (bb_gt[2] - bb_gt[0]) * (bb_gt[3] - bb_gt[1]) - wh
    )
    return o


class Track:
    count = 0

    def __init__(self, bbox):
        self.kf = KalmanFilter(dim_x=7, dim_z=4)
        self.kf.x[:4] = bbox.reshape((4, 1))
        self.id = Track.count
        Track.count += 1
        self.time_since_update = 0
        self.hits = 1
        self.hit_streak = 1
        self.age = 0
        self.bbox = bbox

    def predict(self):
        self.kf.predict()
        self.age += 1
        self.time_since_update += 1
        self.bbox = self.kf.x[:4].reshape((4,))
        return self.bbox

    def update(self, bbox):
        self.time_since_update = 0
        self.hits += 1
        self.hit_streak += 1
        self.kf.update(bbox)
        self.bbox = bbox


class ByteTracker:
    def __init__(self, iou_threshold=0.3, max_age=30):
        self.iou_threshold = iou_threshold
        self.max_age = max_age
        self.tracks = []

    def update(self, detections):
        for t in self.tracks:
            t.predict()

        if len(detections) == 0:
            self.tracks = [t for t in self.tracks if t.time_since_update < self.max_age]
            return self.tracks

        iou_matrix = np.zeros((len(self.tracks), len(detections)), dtype=np.float32)

        for t, track in enumerate(self.tracks):
            for d, det in enumerate(detections):
                iou_matrix[t, d] = iou(track.bbox, det)

        matched_idx = linear_sum_assignment(-iou_matrix)
        matched = []

        for t, d in zip(*matched_idx):
            if iou_matrix[t, d] >= self.iou_threshold:
                self.tracks[t].update(detections[d])
                matched.append(d)

        unmatched_dets = [i for i in range(len(detections)) if i not in matched]

        for d in unmatched_dets:
            self.tracks.append(Track(detections[d]))

        self.tracks = [t for t in self.tracks if t.time_since_update < self.max_age]
        return self.tracks
