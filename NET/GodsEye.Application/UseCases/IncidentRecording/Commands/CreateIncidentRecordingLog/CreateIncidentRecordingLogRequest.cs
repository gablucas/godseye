using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.CreateIncidentRecordingLog
{
    public sealed record CreateIncidentRecordingLogRequest(int cameraId, DateTime incidentTime) : IRequest<ApiResponse<ProcedureResult>>;
}
