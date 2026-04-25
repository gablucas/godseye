using GodsEye.API.Interfaces;
using GodsEye.API.Messages;
using MassTransit;


namespace GodsEye.API.Consumers
{
    public class ExtractedEmbeddingConsumer : IConsumer<ExtractedEmbeddingEvent>
    {
        private readonly IGodsEyeService _godsEyeService;

        public ExtractedEmbeddingConsumer(IGodsEyeService godsEyeService)
        {
            _godsEyeService = godsEyeService;
        }

        public async Task Consume(ConsumeContext<ExtractedEmbeddingEvent> context)
        {
            var message = context.Message;
            await _godsEyeService.ProcessingEmbedding(message.CameraId, message.Embedding, message.IdentifiedAt);
        }
    }
}
