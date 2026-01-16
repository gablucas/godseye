using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog
{
    public sealed record CreateEnvironmentMonitoringLogRequest(int cameraId, int personId, decimal score, DateTime createdAt) : IRequest<ApiResponse<ProcedureResult>>;
}
