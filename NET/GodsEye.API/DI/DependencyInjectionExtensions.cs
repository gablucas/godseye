using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.API.Services.Queries;
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


        }
    }
}
