using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetAllIncidentRecordingLogs
{
    public sealed record GetAllIncidentRecordingLogsRequest(int pageNumber, int pageSize) : IRequest<ApiResponse<IEnumerable<IncidentRecordingModel>>>;
}
