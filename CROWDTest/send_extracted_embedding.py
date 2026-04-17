import pika
import json # Use o json padrão do python ou o do pydantic se preferir
from extracted_embedding import ExtractedEmbedding

class SendExtractedEmbedding:

    def __init__(self):
        # Configuração da conexão
        credentials = pika.PlainCredentials('guest', 'guest')
        parameters = pika.ConnectionParameters('localhost', 5672, '/', credentials)
        self.connection = pika.BlockingConnection(parameters)
        self.channel = self.connection.channel()

    def send_extracted_embedding(self, payload: ExtractedEmbedding):
        # 1. Converte o modelo Pydantic para dicionário e depois para JSON string
        # Se estiver usando Pydantic V2: payload.model_dump()
        # Se estiver usando Pydantic V1: payload.dict()
        message_body = payload.model_dump_json(by_alias=True)

        self.channel.basic_publish(
            exchange='app-exchange',
            routing_key='embedding.created',
            body=message_body,
            properties=pika.BasicProperties(
                content_type='application/json',
                delivery_mode=pika.DeliveryMode.Persistent
            )
        )

        print(f" [x] Sent embedding for camera {payload.camera_id}")

    def close(self):
        """É boa prática ter um método separado para fechar a conexão"""
        if self.connection.is_open:
            self.connection.close()