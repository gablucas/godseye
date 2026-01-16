using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IIncidentRecordingLogRepository
    {
        Task<ProcedureResult> Create(int cameraId, DateTime incidentTime);
        Task<ProcedureResult> Update(int id, int personId);
    }
}
