using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.GetCameraConfigDwellTimeMonitoring
{
    public sealed record  GetCameraConfigDwellTimeMonitoringRequest(int CameraId) : IRequest<ApiResponse<CameraConfigDwellTimeMonitoringModel>>;
}
