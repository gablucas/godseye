using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface ICameraQuery
    {
        Task<IEnumerable<DeviceCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
