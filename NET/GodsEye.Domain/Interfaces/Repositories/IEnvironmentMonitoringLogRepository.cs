using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IEnvironmentMonitoringLogRepository
    {
        Task<ProcedureResult> Create(int cameraId, int userId, decimal score);
    }
}
