import pika
import time
from schemas.extracted_embedding import ExtractedEmbedding

class SendExtractedEmbedding:

    def __init__(self):
        self._connection = None
        self._channel = None

    def _connect(self):
        credentials = pika.PlainCredentials('guest', 'guest')
        parameters = pika.ConnectionParameters(
            'localhost', 5672, '/',
            credentials,
            heartbeat=60,              # mantém conexão viva
            blocked_connection_timeout=30
        )
        self._connection = pika.BlockingConnection(parameters)
        self._channel = self._connection.channel()

    def _ensure_connected(self):
        try:
            if self._connection and self._connection.is_open:
                return
        except Exception:
            pass
        self._connect()

    def send_extracted_embedding(self, payload: ExtractedEmbedding):
        message_body = payload.model_dump_json(by_alias=True)

        for attempt in range(3):
            try:
                self._ensure_connected()
                self._channel.basic_publish(
                    exchange='app-exchange',
                    routing_key='embedding.created',
                    body=message_body,
                    properties=pika.BasicProperties(
                        content_type='application/json',
                        delivery_mode=pika.DeliveryMode.Persistent
                    )
                )
                print(f"[x] Sent embedding for camera {payload.camera_id}")
                return
            except Exception as e:
                print(f"[!] Erro ao publicar (tentativa {attempt+1}): {e}")
                self._connection = None
                self._channel = None
                time.sleep(1)

        print(f"[✗] Falhou ao enviar embedding após 3 tentativas")

    def close(self):
        try:
            if self._connection and self._connection.is_open:
                self._connection.close()
        except Exception:
            pass