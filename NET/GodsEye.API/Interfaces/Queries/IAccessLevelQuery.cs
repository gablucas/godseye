using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IAccessLevelQuery
    {
        Task<IEnumerable<AccessLevelCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
