using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog
{
    public class CreateEnvironmentMonitoringLogHandler : IRequestHandler<CreateEnvironmentMonitoringLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly INotificationSignalR _notification;
        private readonly IApplicationDbContext _context;

        public CreateEnvironmentMonitoringLogHandler(INotificationSignalR notification, IApplicationDbContext context)
        {
            _notification = notification;
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateEnvironmentMonitoringLogRequest request, CancellationToken cancellationToken)
        {
            
            var result = await Create(request.cameraId, request.personId, request.score, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var environmentMonitoringLog = await Get(result.Id, cancellationToken);

            await _notification.SendCreatedEnvironmentMonitoringLog(environmentMonitoringLog);

            return ApiResponse<ProcedureResult>.Ok(result);
        }

        private async Task<ProcedureResult?> Create(int cameraId, int personId, decimal score, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_CREATE_LOG(@P_CAMERA_ID, @P_PERSON_ID, @P_SCORE)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId,
                P_PERSON_ID = personId,
                P_SCORE = score,
            };

            return await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }

        private async Task<EnvironmentMonitoringModel?> Get(int personId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_ID(@P_ID)";

            var parameteres = new
            {
                P_ID = personId
            };

            return await _context.QuerySingleSqlAsync<EnvironmentMonitoringModel>(sql, parameteres, cancellationToken);
        }
    }
}
