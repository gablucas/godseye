from multiprocessing import Process, Event
import queue
import traceback

class LogWorker(Process):
    def __init__(self, log_queue):
        super().__init__()
        self.log_queue = log_queue
        self.stop_event = Event()

    def stop(self):
        self.stop_event.set()

    def run(self):
        print("[LogWorker] iniciado")

        while not self.stop_event.is_set():
            try:
                fn, payload = self.log_queue.get(timeout=1)
            except queue.Empty:
                continue

            try:
                fn(payload)
            except Exception as e:
                print("[LogWorker] erro ao enviar log:", e)
                traceback.print_exc()
