using GodsEye.API.DTO;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared;
using MediatR;

namespace GodsEye.API.Features.EnvironmentMonitoring
{
    internal sealed class CreateEnvironmentMonitoringLogHandler(IDapperContext context, INotificationSignalR signalR, ILogger<CreateEnvironmentMonitoringLogHandler> logger) : INotificationHandler<EnvironmentMonitoringNotification>
    {
        public async Task Handle(EnvironmentMonitoringNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Create(notification.CameraId, notification.PersonId, notification.Score, notification.IdentifiedAt, CancellationToken.None);

                if (result is null || result.Erro == 1)
                    throw new InvalidOperationException("Falha ao registrar log no banco de dados");

                var timeDiff = DateTime.Now - notification.IdentifiedAt;

                if (timeDiff.TotalSeconds < 30)
                {

                    var environmentMonitoringLog = await GetById(result.Id, cancellationToken);

                    if (environmentMonitoringLog != null)
                    {
                        await signalR.SendCreatedEnvironmentMonitoringLog(environmentMonitoringLog);
                    }
                }
            }
            catch (Exception)
            {
                // Opcional: Se o banco falhar, você pode querer "resetar" o cache da pessoa, 
                // mas em sistemas de monitoramento, geralmente é aceitável manter o cache 
                // para evitar spam de tentativas em caso de erro de banco.
                throw;
            }
        }

        protected async Task<ProcedureResult?> Create(int cameraId, int personId, float score, DateTime extractedAt, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_CREATE_LOG(@P_CAMERA_ID, @P_PERSON_ID, @P_SCORE, @P_IDENTIFY_DATE)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId,
                P_PERSON_ID = personId,
                P_SCORE = score,
                P_IDENTIFY_DATE = extractedAt,
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }

        public async Task<EnvironmentMonitoringDTO?> GetById(int personId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_ID(@P_ID)";

            var parameteres = new
            {
                P_ID = personId
            };

            return await context.QuerySingleSqlAsync<EnvironmentMonitoringDTO>(sql, parameteres, cancellationToken);
        }
    }
}
