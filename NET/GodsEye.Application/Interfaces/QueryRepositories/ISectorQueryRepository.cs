using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface ISectorQueryRepository
    {
        Task<IEnumerable<SectorModel>> GetAll(CancellationToken cancellationToken);
    }
}
