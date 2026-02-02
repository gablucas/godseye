using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringLogsByPersonId
{
    public sealed record GetEnvironmentMonitoringLogsByPersonIdRequest(int personId) : IRequest<ApiResponse<EnvironmentMonitoringPersonModel>>;
}
