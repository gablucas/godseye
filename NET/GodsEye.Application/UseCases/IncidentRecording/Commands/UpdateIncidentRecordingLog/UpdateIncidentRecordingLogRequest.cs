using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public sealed record UpdateIncidentRecordingLogRequest(int id, int personId) : IRequest<ApiResponse<ProcedureResult>>;
}
