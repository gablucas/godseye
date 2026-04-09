using GodsEye.Application.Interfaces;
using GodsEye.Application.Messages;
using MassTransit;


namespace GodsEye.Application.Consumers
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
