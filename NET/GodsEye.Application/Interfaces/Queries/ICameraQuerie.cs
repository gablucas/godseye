using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface ICameraQuerie
    {
        Task<IEnumerable<CameraCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
