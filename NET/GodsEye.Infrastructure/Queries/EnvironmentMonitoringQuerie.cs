using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Infrastructure.Queries
{
    public class EnvironmentMonitoringQuerie : IEnvironmentMonitoringQuerie
    {
        private readonly IDapperContext _context;

        public EnvironmentMonitoringQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<EnvironmentMonitoringModel?> GetById(int personId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_ID(@P_ID)";

            var parameteres = new
            {
                P_ID = personId
            };

            return await _context.QuerySingleSqlAsync<EnvironmentMonitoringModel>(sql, parameteres, cancellationToken);
        }
    }
}
