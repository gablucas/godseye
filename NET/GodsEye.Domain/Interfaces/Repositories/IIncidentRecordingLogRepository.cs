using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IIncidentRecordingLogRepository
    {
        Task<ProcedureResult> Create(string macAddress, DateTime incidentTime);
        Task<ProcedureResult> Update(int id, int personId);
    }
}
