using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringPersonsLastSector
{
    public sealed record GetEnvironmentMonitoringLastRegisterPerPerson() : IRequest<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>;
}
