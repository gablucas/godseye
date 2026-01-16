using GodsEye.Application.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;

namespace GodsEye.API.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddAPI(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    ["application/octet-stream"]);
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
        }
    }
}
