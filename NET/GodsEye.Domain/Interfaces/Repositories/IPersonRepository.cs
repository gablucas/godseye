
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IPersonRepository
    {
        Task<ProcedureResult> Create(PersonEntity person, CancellationToken cancellationToken);
    }
}
