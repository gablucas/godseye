using Dapper;

using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.Shared;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Interfaces;
namespace GodsEye.API.DI
{
    public static class DapperDI
    {
        public static void AddDapperDI(this IServiceCollection services)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddScoped<IDapperContext, DapperContext>();

            // VARCHAR TO ENUM
            SqlMapper.AddTypeHandler(new EnumTypeHandler<CompliancePolicyEnum>());
            SqlMapper.AddTypeHandler(new EnumTypeHandler<ComplianceViolationEnum>());

            // EMBEDDIG VARBINARY(2048) TO FLOAT[]
            SqlMapper.AddTypeHandler(new FloatArrayHandler());

            // JSON TO OBJECT
            RegisterJsonTypeHandlers();
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
