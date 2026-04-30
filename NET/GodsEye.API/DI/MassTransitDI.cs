using GodsEye.API.RabbitMQ.Consumers;
using GodsEye.API.RabbitMQ.Messages;
using MassTransit;

namespace GodsEye.API.DI
{
    public static class MassTransitDI
    {
        public static void AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ExtractedEmbeddingConsumer>();
                //x.AddConsumer<ExtractedEmbeddingConsumerBatch>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseRawJsonSerializer();

                    // 1. Define o NOME da exchange atrelada a este evento
                    cfg.Message<ExtractedEmbeddingEvent>(m =>
                    {
                        m.SetEntityName("app-exchange");
                    });

                    // 2. Define o TIPO da exchange na hora de publicar
                    cfg.Publish<ExtractedEmbeddingEvent>(p =>
                    {
                        p.ExchangeType = "direct";
                    });

                    // 3. CRUCIAL: Define a Routing Key que será usada quando alguém publicar este evento
                    cfg.Send<ExtractedEmbeddingEvent>(s =>
                    {
                        s.UseRoutingKeyFormatter(ctx => "embedding.created");
                    });

                    cfg.ReceiveEndpoint("extracted-embedding-queue", e =>
                    {
                        e.ConcurrentMessageLimit = 1;
                        e.PrefetchCount = 1;

                        // 4. Desabilita a topologia automática do MassTransit para este endpoint.
                        // Isso impede a criação de bindings extras indesejados e força o uso do seu bind manual.
                        e.ConfigureConsumeTopology = false;

                        e.ConfigureConsumer<ExtractedEmbeddingConsumer>(context);
                        //e.ConfigureConsumer<ExtractedEmbeddingConsumerBatch>(context);

                        // 5. Faz o bind manual da sua fila diretamente com a "app-exchange"
                        e.Bind("app-exchange", b =>
                        {
                            b.ExchangeType = "direct";
                            b.RoutingKey = "embedding.created";
                        });

                        //e.Batch<ExtractedEmbeddingEvent>(b =>
                        //{
                        //    b.MessageLimit = 50;
                        //    b.TimeLimit = TimeSpan.FromSeconds(1);

                        //    // Aqui você passa **o consumidor via DI**
                        //    b.Consumer(() => context.GetRequiredService<ExtractedEmbeddingConsumerBatch>());
                        //});
                    });
                });
            });
        }
    }
}
