using Dapper;

using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.Shared;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Interfaces;
using GodsEye.Shared.Response.AccessLevel;
namespace GodsEye.API.DI
{
    public static class DapperDI
    {
        public static void AddDapperDI(this IServiceCollection services)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddScoped<IDapperContext, DapperContext>();
            RegisterJsonTypeHandlers();

            SqlMapper.AddTypeHandler(new EnumTypeHandler<ComplianceRuleEnum>());
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<SectorAccessLevelDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelScheduleRuleDTO>());
            SqlMapper.AddTypeHandler(new FloatArrayHandler());
        }

        private static void RegisterJsonTypeHandlers()
        {
            var handlerType = typeof(JsonTypeHandler<>);

            var assemblies = new[]
            {
                typeof(ISharedLayer).Assembly,
                typeof(IApiLayer).Assembly,
            };

            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .ToList();

                // IJsonType → registra T e List<T>
                foreach (var type in allTypes.Where(t => typeof(IJsonType).IsAssignableFrom(t)))
                {
                    RegisterHandler(handlerType, type);
                    RegisterHandler(handlerType, typeof(List<>).MakeGenericType(type));
                }

                // IJsonTypeList → registra APENAS List<T>
                foreach (var type in allTypes.Where(t => typeof(IJSonTypeList).IsAssignableFrom(t)))
                {
                    RegisterHandler(handlerType, typeof(List<>).MakeGenericType(type));
                }
            }
        }

        private static void RegisterHandler(Type handlerType, Type type)
        {
            var concrete = handlerType.MakeGenericType(type);
            var instance = (SqlMapper.ITypeHandler)Activator.CreateInstance(concrete)!;
            SqlMapper.AddTypeHandler(type, instance);
        }
    }
}
