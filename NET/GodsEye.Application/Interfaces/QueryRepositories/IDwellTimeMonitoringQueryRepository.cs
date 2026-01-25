using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IDwellTimeMonitoringQueryRepository
    {
        Task<List<DwellTimeMonitoringModel>> GetAll(CancellationToken cancellationToken);
    }
}
