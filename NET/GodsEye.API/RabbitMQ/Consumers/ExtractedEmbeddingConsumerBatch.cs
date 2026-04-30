using GodsEye.API.RabbitMQ.Messages;
using MassTransit;

namespace GodsEye.API.RabbitMQ.Consumers
{
    public class ExtractedEmbeddingConsumerBatch : IConsumer<Batch<ExtractedEmbeddingEvent>>
    {
        public Task Consume(ConsumeContext<Batch<ExtractedEmbeddingEvent>> context)
        {
            var batch = context.Message;


            foreach (var msgContext in batch)
            {
                // msgContext.Message é a instância real de ExtractedEmbeddingEvent
                var embedding = msgContext.Message;
            }

            return Task.CompletedTask;
        }
    }
}
