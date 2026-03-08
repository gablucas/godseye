using Dapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Infrastructure.Email;
using GodsEye.Infrastructure.MediaMtx;
using GodsEye.Infrastructure.Persistence;
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
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<ICameraConnectionTesterService, RtspCameraConnectionTesterService>();


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
        }
    }
}
