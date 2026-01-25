using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class DwellTimeMonitoringQueryRepository : IDwellTimeMonitoringQueryRepository
    {
        private readonly AppDbContext _context;

        public DwellTimeMonitoringQueryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<DwellTimeMonitoringModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.DwellTimeMonitoringModel
                .FromSqlRaw("CALL SP_DWELL_TIME_MONITORING_GET_ALL()")
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
