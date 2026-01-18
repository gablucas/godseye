using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IFeatureRepository
    {
        Task<IReadOnlyCollection<FeatureEntity>> GetAll();
    }
}
