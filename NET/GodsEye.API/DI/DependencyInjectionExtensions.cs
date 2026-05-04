using FluentValidation;
using GodsEye.API.Email;
using GodsEye.API.Features.Compliance.SectorTransition;
using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Features.Compliance.Violation;
using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.API.Services.Queries;
using Hangfire;
using Hangfire.MySql;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.ResponseCompression;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace GodsEye.API.DI
{
    public static class DependencyInjectionExtensions
    {
        public static void AddAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    ["application/octet-stream"]);
            });

            QuestPDF.Settings.License = LicenseType.Community;

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


            services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy
                        .WithOrigins("https://localhost:7198", "http://localhost:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddSingleton<INotificationSignalR, SignalRRealTimeNotification>();
            services.RegisterEndpointsFromAssemblyContaining<IApiMarker>();
            services.AddValidatorsFromAssemblyContaining<IApiMarker>();
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailService, MailKitEmailSender>();



            services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseRedisStorage("localhost:6379", new RedisStorageOptions
            {
                Prefix = "hangfire:",
                ExpiryCheckInterval = TimeSpan.FromHours(1),
                InvisibilityTimeout = TimeSpan.FromMinutes(30)
            }));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 20; // pode aumentar sem medo, Redis aguenta
            });

            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<ICameraConnectionTesterService, RtspCameraConnectionTesterService>();

            services.AddSingleton<IFaceMatcherService, FaceMatcherService>();
            services.AddSingleton<IGodsEyeState, GodsEyeState>();
            services.AddScoped<IComplianceStrategy, SectorTransitionStrategy>();
            services.AddScoped<IComplianceLogService, ComplianceLogService>();
            services.AddScoped<IComplianceViolationService, ComplianceViolationService>();

            services.AddScoped<IPersonQuery, PersonQuery>();
            services.AddScoped<ICameraQuery, CameraQuery>();
            services.AddScoped<IAccessLevelQuery, AccessLevelQuery>();
            services.AddScoped<ISectorTransitionQuery, SectorTransitionQuery>();
            services.AddScoped<IComplianceViolationQuery, ComplianceViolationQuery>();
            
        }
    }
}
