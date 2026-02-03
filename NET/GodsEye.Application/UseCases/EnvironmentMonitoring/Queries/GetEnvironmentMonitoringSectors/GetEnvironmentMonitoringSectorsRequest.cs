using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringSectors
{
    public sealed record GetEnvironmentMonitoringSectorsRequest() : IRequest<ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>>;
}
