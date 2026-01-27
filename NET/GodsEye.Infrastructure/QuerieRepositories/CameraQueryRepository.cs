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

        public async Task<CameraModel> GetById(int cameraId, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);

            var result = await _context.CameraModel
                .FromSqlRaw("CALL SP_CAMERA_GET_BY_ID(@P_CAMERA_ID)", pCameraId)
                .ToListAsync();

            return result.FirstOrDefault() ?? new CameraModel();
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

        public async Task<IEnumerable<CameraByFeatureModel>> GetCamerasByFeatureId(int featureId, CancellationToken cancellationToken)
        {
            var pFeatureId = new MySqlParameter("@P_FEATURE_ID", featureId);

            var result = await _context.CameraByFeatureModel
                .FromSqlRaw("CALL SP_CAMERA_GET_BY_FEATURE_ID(@P_FEATURE_ID)", pFeatureId)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<IEnumerable<CameraFeatureModel>> GetCameraFeaturesById(int cameraId, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);

            var result = await _context.CameraFeatureModel
                .FromSqlRaw("CALL SP_CAMERA_GET_FEATURES_BY_CAMERA_ID(@P_CAMERA_ID)", pCameraId)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
