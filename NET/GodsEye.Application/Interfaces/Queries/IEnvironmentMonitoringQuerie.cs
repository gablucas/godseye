using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IEnvironmentMonitoringQuerie
    {
        Task<EnvironmentMonitoringModel?> GetById(int personId, CancellationToken cancellationToken);
    }
}
