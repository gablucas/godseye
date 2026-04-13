using Dapper;
using GodsEye.Application.Consumers;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using GodsEye.Application.Interfaces.Write;
using GodsEye.Application.Messages;
using GodsEye.Infrastructure.Email;
using GodsEye.Infrastructure.MediaMtx;
using GodsEye.Infrastructure.Persistence;
using GodsEye.Infrastructure.Queries;
using GodsEye.Infrastructure.Write;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GodsEye.Infrastructure.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IFaceMatcherService, FaceMatcherService>();
            services.AddSingleton<IGodsEyeState, GodsEyeState>();

            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<ICameraConnectionTesterService, RtspCameraConnectionTesterService>();

            services.AddScoped<IPersonQuerie, PersonQuerie>();
            services.AddScoped<ICameraQuerie, CameraQuerie>();
            services.AddScoped<IEnvironmentMonitoringQuerie, EnvironmentMonitoringQuerie>();
            services.AddScoped<IRoutineQuerie, RoutineQuerie>();

            services.AddScoped<IEnvironmentMonitoringWrite, EnvironmentMonitoringWrite>();




            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(
                    configuration.GetConnectionString("MySQLConnection"),
                    new MySqlServerVersion(new Version(8, 0, 44))
                );
            });

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            services.AddHttpClient<IGodsEyeService, GodsEyeService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["GodsEye:BaseUrl"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("A URL de GodsEye não foi configurada.");

                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            });

            services.AddOptions<MediaMtxOptions>()
            .Bind(configuration.GetSection("MediaMtx"))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.ApiBaseUrl) &&
                !string.IsNullOrWhiteSpace(o.WebRtcBaseUrl),
                "Configuração do MediaMTX inválida"
            )
            .ValidateOnStart();

            services.AddHttpClient<IMediaMtxService, MediaMtxService>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<MediaMtxOptions>>()
                    .Value;


                if (string.IsNullOrWhiteSpace(options.ApiBaseUrl))
                    throw new InvalidOperationException("A URL do MediaMtx não foi configurada.");

                client.BaseAddress = new Uri(options.ApiBaseUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            });

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

            services.AddScoped<IEmailService, MailKitEmailSender>();

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


            services.AddScoped<IDapperContext, DapperContext>();

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EnvironmentMonitoringPersonLog>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EnvironmentMonitoringModel>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<IncidentRecordingPersonDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EmailDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<CameraDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<NotificationGroupDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessScheduleRuleModel>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<SectorDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<RoiModel>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<SectorAccessLevelDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleRuleDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureCache>>());

            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<CameraCache>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<RoutineRuleSectorTransitionModel>>());

            SqlMapper.AddTypeHandler(new FloatArrayHandler());
        }
    }
}
