using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IEnvironmentMonitoringQueryRepository
    {
        Task<EnvironmentMonitoringModel> GetByLogId(int logId, CancellationToken cancellationToken);
    }
}
