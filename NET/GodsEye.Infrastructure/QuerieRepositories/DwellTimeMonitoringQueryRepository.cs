using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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

        public async Task<List<DwellTimeMonitoringDetailsModel>> GetDetailsByCameraId(int cameraId, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);

            var result = await _context.DwellTimeMonitoringDetailsModel
                .FromSqlRaw("CALL SP_DWELL_TIME_MONITORING_GET_DETAILS_BY_CAMERA_ID(@P_CAMERA_ID)", pCameraId)
                .ToListAsync();

            return result;
        }
    }
}
