using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraConfigIncidentRecording
{
    public sealed record GetCameraConfigIncidentRecordingRequest(int CameraId) : IRequest<ApiResponse<CameraConfigIncidentRecordingModel>>;
}
