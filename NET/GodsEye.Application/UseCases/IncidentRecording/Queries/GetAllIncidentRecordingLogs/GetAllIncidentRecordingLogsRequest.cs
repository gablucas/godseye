using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetAllIncidentRecordingLogs
{
    public class GetAllIncidentRecordingLogsRequest() : IRequest<ApiResponse<IEnumerable<IncidentRecordingModel>>>;
}
