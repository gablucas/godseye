using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public sealed record UpdateIncidentRecordingLogRequest(int incidentId, List<int> personIds, string videoPath) : IRequest<ApiResponse<ProcedureResult>>;
}
