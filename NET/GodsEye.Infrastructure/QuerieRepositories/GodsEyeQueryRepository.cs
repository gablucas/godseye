using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class GodsEyeQueryRepository : IGodsEyeQueryRepository
    {
        private readonly AppDbContext _context;

        public GodsEyeQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MonitoringDataModel> GetMonitoringData()
        {
            var result = await _context.MonitoringDataModel
                .FromSqlRaw("CALL SP_ENVIRONMENT_MONITORING_GET_DATA()")
                .ToListAsync();

            return result.FirstOrDefault() ?? new MonitoringDataModel();
        }
    }
}
