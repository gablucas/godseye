using Dapper;
using GodsEye.API.DTO;
using GodsEye.API.Interfaces;
using GodsEye.API.Services;
using GodsEye.Shared.Response.AccessLevel;
using GodsEye.Shared.Response.AccessSchedule;
using GodsEye.Shared.Response.Camera;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.Shared.Response.NotificationGroups;
using GodsEye.Shared.Response.Sector;

namespace GodsEye.API.DI
{
    public static class DapperDI
    {
        public static void AddDapperDI(this IServiceCollection services)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddScoped<IDapperContext, DapperContext>();
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<IncidentRecordingPersonDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EnvironmentMonitoringPersonLogResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<SectorDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AccessLevelDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureCache>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessLevelSectorRuleCache>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessViolationEmailsDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EnvironmentMonitoringLogResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<CameraSectorResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<NotificationGroupResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<FeatureResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<RoiDTO>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<PointDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<SectorAccessLevelDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessLevelScheduleRuleDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessScheduleResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessScheduleRuleDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<NotificationGroupsResponse>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<EmailDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<SectorDTO>>());
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<AccessLevelDTO>>());
        }
    }
}
