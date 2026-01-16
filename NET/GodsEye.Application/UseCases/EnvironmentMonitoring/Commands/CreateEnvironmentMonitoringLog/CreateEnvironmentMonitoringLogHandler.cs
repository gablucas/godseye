using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog
{
    public class CreateEnvironmentMonitoringLogHandler : IRequestHandler<CreateEnvironmentMonitoringLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly INotificationSignalR _notification;
        private readonly IEnvironmentMonitoringLogRepository _environmentMonitoringLogRepository;
        private readonly IEnvironmentMonitoringQueryRepository _environmentMonitoringQueryRepository;

        public CreateEnvironmentMonitoringLogHandler(INotificationSignalR notification, IEnvironmentMonitoringLogRepository environmentMonitoringLogRepository, IEnvironmentMonitoringQueryRepository environmentMonitoringQueryRepository)
        {
            _notification = notification;
            _environmentMonitoringLogRepository = environmentMonitoringLogRepository;
            _environmentMonitoringQueryRepository = environmentMonitoringQueryRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateEnvironmentMonitoringLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _environmentMonitoringLogRepository.Create(request.cameraId, request.personId, request.score);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var environmentMonitoringLog = await _environmentMonitoringQueryRepository.GetByLogId(result.Id, cancellationToken);

            await _notification.SendEnvironmentMonitoringCreatedLog(environmentMonitoringLog);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
