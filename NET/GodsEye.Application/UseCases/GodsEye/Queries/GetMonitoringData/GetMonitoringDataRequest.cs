using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.GodsEye.Queries.GetMonitoringData
{
    public sealed record GetMonitoringDataRequest : IRequest<ApiResponse<MonitoringDataModel>>;
}
