using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IPersonQueries
    {
        Task<PersonModel?> GetById(int personId, CancellationToken cancellationToken);
    }
}
