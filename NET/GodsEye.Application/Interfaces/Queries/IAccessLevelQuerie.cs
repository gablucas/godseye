using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IAccessLevelQuerie
    {
        Task<IEnumerable<AccessLevelCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
