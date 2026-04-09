using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IPersonQuerie
    {
        Task<PersonModel?> GetById(int personId, CancellationToken cancellationToken);
        Task<IEnumerable<PersonCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
