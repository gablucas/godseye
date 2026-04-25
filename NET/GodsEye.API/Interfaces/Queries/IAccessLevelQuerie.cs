using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IAccessLevelQuerie
    {
        Task<IEnumerable<AccessLevelCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
