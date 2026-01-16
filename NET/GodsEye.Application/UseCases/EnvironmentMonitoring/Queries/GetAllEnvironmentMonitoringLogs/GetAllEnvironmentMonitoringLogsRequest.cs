using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs
{
    public sealed record GetAllEnvironmentMonitoringLogsRequest() : IRequest<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>;
}
