using Microsoft.AspNetCore.Builder;

namespace GodsEye.API.Extensions
{
    public static class CorsExtensions
    {
        private const string PolicyName = "AllowBlazor";

        public static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins("https://localhost:7198")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public static void UseCorsPolicy(this IApplicationBuilder app) {
            app.UseCors(PolicyName);
        }
    }
}
