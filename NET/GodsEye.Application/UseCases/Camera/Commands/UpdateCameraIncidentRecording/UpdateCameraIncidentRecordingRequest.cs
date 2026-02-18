using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraIncidentRecording
{
    public sealed record UpdateCameraIncidentRecordingRequest(int CameraId, string MacAddress) : IRequest<ApiResponse<int>>;
}
