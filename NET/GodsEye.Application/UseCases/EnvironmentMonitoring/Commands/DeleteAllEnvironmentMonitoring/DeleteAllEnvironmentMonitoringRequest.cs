using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.DeleteAllEnvironmentMonitoring
{
    public sealed record DeleteAllEnvironmentMonitoringRequest() : IRequest<ApiResponse<bool>>;
}
