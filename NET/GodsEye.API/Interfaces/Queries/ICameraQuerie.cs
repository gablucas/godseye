using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface ICameraQuerie
    {
        Task<IEnumerable<CameraCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
