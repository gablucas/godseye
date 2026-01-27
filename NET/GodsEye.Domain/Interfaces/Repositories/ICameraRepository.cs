using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface ICameraRepository
    {
        Task<ProcedureResult> Create(CameraEntity camera, CancellationToken cancellationToken);
        Task<ProcedureResult> Update(CameraEntity camera, CancellationToken cancellationToken);
    }
}
