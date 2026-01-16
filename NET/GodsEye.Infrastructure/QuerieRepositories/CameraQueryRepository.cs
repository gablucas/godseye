using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class CameraQueryRepository : ICameraQueryRepository
    {
        private readonly AppDbContext _context;

        public CameraQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CameraModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.CameraModel
                .FromSqlRaw("CALL SP_CAMERA_GET_ALL()")
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CameraConnectionModel>> GetAllConnection(CancellationToken cancellationToken)
        {
            var result = await _context.CameraConnectionModel
                .FromSqlRaw("CALL SP_GET_ALL_CAMERA_CONNECTION()")
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CameraLogModel>> GetAllLogs(int cameraId, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);

            var result = await _context.CameraLogModel
                .FromSqlRaw("CALL SP_CAMERA_ENVIRONMENT_GET_MONITORING_LOG(@P_CAMERA_ID)", pCameraId)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
