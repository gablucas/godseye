using Dapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using GodsEye.Application.Interfaces.Write;
using GodsEye.Application.Services;
using GodsEye.Infrastructure.Email;
using GodsEye.Infrastructure.MediaMtx;
using GodsEye.Infrastructure.Persistence;
using GodsEye.Infrastructure.Queries;
using GodsEye.Infrastructure.Write;
using GodsEye.Shared.Response.NotificationGroups;
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

            services.AddScoped<IPersonQuerie, PersonQuerie>();
            services.AddScoped<ICameraQuerie, CameraQuerie>();
            services.AddScoped<IRoutineQuerie, RoutineQuerie>();
            services.AddScoped<IAccessLevelQuerie, AccessLevelQuerie>();
            services.AddScoped<IAccessViolationQuerie, AccessViolationQuerie>();
            services.AddScoped<IComplianceQuerie, ComplianceQuerie>();

            services.AddScoped<IEnvironmentMonitoringWrite, EnvironmentMonitoringWrite>();
            services.AddScoped<IComplianceWrite, ComplianeWrite>();




            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(
                    configuration.GetConnectionString("MySQLConnection"),
                    new MySqlServerVersion(new Version(8, 0, 44))
                );
            });

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());


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


            services.AddSingleton<INotificationSignalR, SignalRRealTimeNotification>();

            services.AddScoped<IDapperContext, DapperContext>();

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<IncidentRecordingPersonDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessScheduleRuleModel>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<SectorDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<RoiModel>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<SectorAccessLevelDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleRuleDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureCache>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessLevelSectorRuleCache>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<RoutineRuleSectorTransitionModel>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessViolationEmailsDTO>>());


            SqlMapper.AddTypeHandler(new FloatArrayHandler());
        }
    }
}
