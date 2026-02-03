using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Threading;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class EnvironmentMonitoringQueryRepository : IEnvironmentMonitoringQueryRepository
    {
        private readonly AppDbContext _context;

        public EnvironmentMonitoringQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EnvironmentMonitoringModel> GetByLogId(int logId, CancellationToken cancellationToken)
        {
            var pLogId = new MySqlParameter("@P_LOG_ID", logId);

            var result = await _context.EnvironmentMonitoringModel
                .FromSqlRaw("CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_ID(@P_LOG_ID)", pLogId)
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault() ?? new EnvironmentMonitoringModel();
        }
    }
}
