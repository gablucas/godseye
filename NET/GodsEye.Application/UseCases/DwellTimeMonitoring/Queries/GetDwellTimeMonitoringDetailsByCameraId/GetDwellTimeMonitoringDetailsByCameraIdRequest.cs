using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetDwellTimeMonitoringDetailsByCameraId
{
    public sealed record GetDwellTimeMonitoringDetailsByCameraIdRequest(int cameraId) : IRequest<ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>>;
}