using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface ISectorRepository
    {
        Task<ProcedureResult> Create(SectorEntity person, CancellationToken cancellationToken);
    }
}
