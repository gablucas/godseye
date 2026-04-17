using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using GodsEye.Application.Interfaces.Write;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog
{
    public class CreateEnvironmentMonitoringLogHandler : IRequestHandler<CreateEnvironmentMonitoringLogRequest, Unit>
    {
        private readonly INotificationSignalR _notification;
        private readonly IEnvironmentMonitoringQuerie _environmentMonitoringQuerie;
        private readonly IEnvironmentMonitoringWrite _environemntMonitoringWrite;

        public CreateEnvironmentMonitoringLogHandler(INotificationSignalR notification, IEnvironmentMonitoringQuerie environmentMonitoringQuerie, IEnvironmentMonitoringWrite environemntMonitoringWrite)
        {
            _notification = notification;
            _environmentMonitoringQuerie = environmentMonitoringQuerie;
            _environemntMonitoringWrite = environemntMonitoringWrite;
        }

        public async Task<Unit> Handle(CreateEnvironmentMonitoringLogRequest request, CancellationToken cancellationToken)
        {
           
            try
            {
                var result = await _environemntMonitoringWrite.Create(request.cameraId, request.personId, request.score, request.identifiedAt, CancellationToken.None);

                if (result is null || result.Erro == 1)
                    throw new InvalidOperationException("Falha ao registrar log no banco de dados");

                var timeDiff = DateTime.Now - request.identifiedAt;

                if (timeDiff.TotalSeconds < 30) {

                    var environmentMonitoringLog = await _environmentMonitoringQuerie.GetById(result.Id, cancellationToken);

                    if (environmentMonitoringLog != null)
                    {
                        await _notification.SendCreatedEnvironmentMonitoringLog(environmentMonitoringLog);
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

            return Unit.Value;
        }
    }
}
