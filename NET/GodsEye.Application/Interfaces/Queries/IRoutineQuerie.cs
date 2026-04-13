using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IRoutineQuerie
    {
        Task<RoutineModel?> GetById(int routineId, CancellationToken cancellationToken);
    }
}
