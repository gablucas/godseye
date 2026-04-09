using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Application.Interfaces.Write
{
    public interface IEnvironmentMonitoringWrite
    {
        Task<ProcedureResult?> Create(int cameraId, int personId, float score, DateTime identifiedAt, CancellationToken cancellationToken);
    }
}
