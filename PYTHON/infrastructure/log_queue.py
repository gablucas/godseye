from queue import Queue
import threading
import asyncio
import inspect

log_queue = Queue()
_worker_started = False

def log_worker():
    while True:
        fn, payload = log_queue.get()
        try:
            if inspect.iscoroutinefunction(fn):
                result = asyncio.run(fn(payload))
            else:
                result = fn(payload)

            if result is not None:
                print(result)

        except Exception as e:
            print(f"[LOG WORKER] Erro ao enviar log: {e}")
        finally:
            log_queue.task_done()

def start_log_worker():
    global _worker_started
    if _worker_started:
        return
    _worker_started = True
    threading.Thread(target=log_worker, daemon=True).start()
