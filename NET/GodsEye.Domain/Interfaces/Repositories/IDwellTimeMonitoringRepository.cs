using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IDwellTimeMonitoringRepository
    {
        Task<ProcedureResult> Create(DwellTimeMonitoringEntity dwellTimeMonitoring, CancellationToken cancellationToken);
        Task<ProcedureResult> Update(DwellTimeMonitoringEntity dwellTimeMonitoring, CancellationToken cancellationToken);
    }
}
