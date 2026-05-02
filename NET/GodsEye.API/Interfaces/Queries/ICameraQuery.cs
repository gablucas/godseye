using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface ICameraQuery
    {
        Task<IEnumerable<CameraCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
