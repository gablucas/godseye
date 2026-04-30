from multiprocessing import Queue, Manager
from services.camera_process import CameraProcess
from workers.inference_worker import InferenceWorker
from infrastructure.log_worker import LogWorker
from utils.has_feature import has_feature

class MonitorManager:
    def __init__(self, cameras):
        self.cameras = cameras

        self.workers = {}
        self.manager = Manager()
        self.log_queue = Queue(maxsize=2000)
        self.log_worker = LogWorker(self.log_queue)

    def start_monitoring(self):

        self.log_worker.start()

        for cam in self.cameras:
            print("##############################################################")
            print(f"INICIANDO MONITORAMENTO DA CAMERA {cam['Id']}")

            features = self.allowed_features(cam)

            print("##############################################################")

            process = CameraProcess(
                camera_id=cam["Id"],
                rtsp_url=cam["Connection"],
                sector_id=cam["SectorId"],
                roi=cam["Roi"],
                features=features,
                log_queue=self.log_queue
            )

            process.start()
            self.workers[cam["Id"]] = process

    def allowed_features(self, cam):
        return {
            "environment_monitoring": has_feature(cam, 1),
            "incident_recording": has_feature(cam, 2),
            "dwell_time_monitoring": has_feature(cam, 3)
        }

    def stop_camera(self, camera_id):
        # 1. Usa .pop() para remover do dicionário E pegar o processo ao mesmo tempo
        # O segundo parametro None evita erro se o ID não existir
        process = self.workers.pop(camera_id, None)
        
        if process:
            print(f"🛑 Parando Câmera {camera_id}...")
            process.stop() # Sinaliza o evento de parada
            
            # 2. Join com Timeout (Segurança contra travamentos)
            process.join(timeout=5)

            # 3. Se ainda estiver vivo após 5s (travou), mata forçado
            if process.is_alive():
                print(f"⚠️ Câmera {camera_id} travou no encerramento. Forçando kill...")
                process.terminate()
                process.join() # Limpa os recursos do processo morto
            
            print(f"✅ Câmera {camera_id} encerrada.")

    def stop_all(self):
                print("🛑 Iniciando parada geral...")
                
                # Cria uma lista dos processos para não iterar sobre o dicionário enquanto removemos
                # Note que aqui só sinalizamos o stop para todos PRIMEIRO (paralelismo)
                processes_to_stop = list(self.workers.values()),

                for process in processes_to_stop:
                    process.stop()

                for process in processes_to_stop:
                    process.join(timeout=3)

                # Limpa a referência
                self.workers.clear()

                # Para o Worker de Log
                if hasattr(self, 'log_worker') and self.log_worker.is_alive():
                    print("🛑 Parando LogWorker")
                    self.log_worker.stop() # Certifique-se que sua classe LogWorker tem esse método
                    self.log_worker.join(timeout=5)
                    if self.log_worker.is_alive():
                        self.log_worker.terminate()

                # IMPORTANTE: Encerra o Manager para liberar memória compartilhada e sockets
                if hasattr(self, 'manager'):
                    print("🧹 Encerrando Manager de memória compartilhada...")
                    self.manager.shutdown()
                    
                print("✅ Monitoramento encerrado completamente.")
