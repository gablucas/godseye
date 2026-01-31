using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Email;
using GodsEye.Infrastructure.GodsEye;
using GodsEye.Infrastructure.Persistence;
using GodsEye.Infrastructure.QuerieRepositories;
using GodsEye.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GodsEye.Infrastructure.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFolderService, FolderService>();

            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ICameraRepository, CameraRepository>();
            services.AddScoped<ISectorRepository, SectorRepository>();
            services.AddScoped<IEnvironmentMonitoringLogRepository, EnvironmentMonitoringLogRepository>();
            services.AddScoped<IIncidentRecordingRepository, IncidentRecordingLogRepository>();
            services.AddScoped<IDwellTimeMonitoringRepository, DwellTimeMonitoringRepository>();
            services.AddScoped<IFeatureRepository, FeatureRepository>();
            services.AddScoped<INotificationGroupRepository, NotificationGroupRepository>();

            services.AddScoped<ICameraQueryRepository, CameraQueryRepository>();
            services.AddScoped<IPersonQueryRepository, PersonQueryRepository>();
            services.AddScoped<ISectorQueryRepository, SectorQueryRepository>();
            services.AddScoped<IGodsEyeQueryRepository, GodsEyeQueryRepository>();
            services.AddScoped<IEnvironmentMonitoringQueryRepository, EnvironmentMonitoringQueryRepository>();
            services.AddScoped<IIncidentRecordingQueryRepository, IncidentRecordingQueryRepository>();
            services.AddScoped<IDwellTimeMonitoringQueryRepository, DwellTimeMonitoringQueryRepository>();
            services.AddScoped<INotificationGroupQueryRepository, NotificationGroupQueryRepository>();


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

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

            services.AddScoped<IEmailService, MailKitEmailSender>();
        }
    }
}
