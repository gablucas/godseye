using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog
{
    public sealed record CreateEnvironmentMonitoringLogRequest(int cameraId, int personId, float score, DateTime identifiedAt) : IRequest<Unit>;
}
