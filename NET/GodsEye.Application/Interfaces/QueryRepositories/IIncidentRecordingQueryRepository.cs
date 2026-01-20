using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IIncidentRecordingQueryRepository
    {
        Task<IEnumerable<IncidentRecordingModel>> GetAll(CancellationToken cancellationToken);
        Task<IncidentRecordingModel> GetByLogId(int logId, CancellationToken cancellationToken);
        Task<IncidentRecordingProcessModel?> GetToProcess(CancellationToken cancellationToken);
    }
}
