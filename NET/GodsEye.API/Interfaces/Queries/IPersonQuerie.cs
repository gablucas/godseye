
using GodsEye.API.DTO;
using GodsEye.Shared.Response.Person;

namespace GodsEye.API.Interfaces
{
    public interface IPersonQuerie
    {
        Task<PersonResponse?> GetById(int personId, CancellationToken cancellationToken);
        Task<IEnumerable<PersonCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
