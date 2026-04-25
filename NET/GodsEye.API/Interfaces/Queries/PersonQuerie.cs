using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IPersonQuerie
    {
        Task<PersonDTO?> GetById(int personId, CancellationToken cancellationToken);
        Task<IEnumerable<PersonCache>> GetAllCache(CancellationToken cancellationToken);
    }
}
