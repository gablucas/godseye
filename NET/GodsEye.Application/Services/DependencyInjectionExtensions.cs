using GodsEye.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GodsEye.Application.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddAplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        }
    }
}
