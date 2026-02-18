using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraConfigDwellTimeMonitoring
{
    public sealed record UpdateCameraConfigDwellTimeMonitoringRequest(int Id, int MaxDwellTimeMinutes, int MaxNonIdentificationTimeMinutes) : IRequest<ApiResponse<int>>;
}
