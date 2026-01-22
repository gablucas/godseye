using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingLogById
{
    public sealed record GetIncidentRecordingLogByIdRequest(int logId) : IRequest<ApiResponse<IncidentRecordingModel>>;
}
