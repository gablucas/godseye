using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface ICameraQueryRepository
    {
        Task<IEnumerable<CameraModel>> GetAll(CancellationToken cancellationToken);
        Task<IEnumerable<CameraConnectionModel>> GetAllConnection(CancellationToken cancellationToken);
        Task<IEnumerable<CameraLogModel>> GetAllLogs(int cameraId, CancellationToken cancellationToken);
    }
}
