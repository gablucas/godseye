using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IEnvironmentMonitoringQueryRepository
    {
        Task<IEnumerable<EnvironmentMonitoringModel>> GetAll(CancellationToken cancellationToken);
        Task<EnvironmentMonitoringModel> GetByLogId(int logId, CancellationToken cancellationToken);
    }
}
