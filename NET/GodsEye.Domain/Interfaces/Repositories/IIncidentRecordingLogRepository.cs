using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.ValueObjects;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface IIncidentRecordingLogRepository
    {
        Task<ProcedureResult> Create(string macAddress, DateTime incidentTime);
        Task<ProcedureResult> Update(int id, List<IncidentRecordingPersonVO> personId, string videoPath);
    }
}
