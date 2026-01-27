using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface ICameraQueryRepository
    {
        Task<IEnumerable<CameraModel>> GetAll(CancellationToken cancellationToken);
        Task<CameraModel> GetById(int cameraId, CancellationToken cancellationToken);
        Task<IEnumerable<CameraConnectionModel>> GetAllConnection(CancellationToken cancellationToken);
        Task<IEnumerable<CameraLogModel>> GetAllLogs(int cameraId, CancellationToken cancellationToken);
        Task<IEnumerable<CameraByFeatureModel>> GetCamerasByFeatureId(int featureId, CancellationToken cancellationToken);
        Task<IEnumerable<CameraFeatureModel>> GetCameraFeaturesById(int cameraId, CancellationToken cancellationToken);
    }
}
