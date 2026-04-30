using FluentValidation;
using GodsEye.API.Features.Compliance.SectorTransition;
using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.API.Services.Queries;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.ResponseCompression;
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
                        .WithOrigins("https://localhost:7198")
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
            services.AddSingleton<IFaceMatcherService, FaceMatcherService>();
            services.AddSingleton<IGodsEyeState, GodsEyeState>();
            

            services.AddScoped<IPersonQuerie, PersonQuerie>();
            services.AddScoped<ICameraQuerie, CameraQuerie>();
            services.AddScoped<IAccessLevelQuerie, AccessLevelQuerie>();

            // ✅ Registrar o Hangfire
            services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(
                configuration.GetConnectionString("HangfireConnection"),
                new MySqlStorageOptions
                {
                    TablesPrefix = "Hangfire_"
                }
            )));

            // ✅ Adicionar o servidor de processamento
            services.AddHangfireServer();

            services.AddScoped<IComplianceStrategy, SectorTransitionStrategy>();
            services.AddScoped<IComplianceLogService, ComplianceLogService>();
            services.AddScoped<IComplianceViolationService, ComplianceViolationService>();
        }
    }
}
