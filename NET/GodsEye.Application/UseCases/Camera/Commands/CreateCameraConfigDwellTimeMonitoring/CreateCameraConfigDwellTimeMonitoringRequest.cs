using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCameraConfigDwellTimeMonitoring
{
    public sealed record CreateCameraConfigDwellTimeMonitoringRequest(int CameraId, int MaxDwellTimeMinutes, int MaxNonIdentificationTimeMinutes) : IRequest<ApiResponse<int>>;
}
