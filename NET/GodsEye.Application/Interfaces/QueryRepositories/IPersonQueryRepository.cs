using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IPersonQueryRepository
    {
        Task<IEnumerable<PersonModel>> GetAll(CancellationToken cancellationToken);
        Task<IEnumerable<PersonEmbeddingModel>> GetAllEmbeddings(CancellationToken cancellationToken);
    }
}
