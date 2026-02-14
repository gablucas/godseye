using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.CreateIncidentRecordingLog
{
    public class CreateIncidentRecordingLogHandler : IRequestHandler<CreateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly INotificationSignalR _notification;
        private readonly IIncidentRecordingRepository _incidentRecordingLogRepository;
        private readonly IIncidentRecordingQueryRepository _incidentRecordingQueryRepository;

        public CreateIncidentRecordingLogHandler(INotificationSignalR notification, IIncidentRecordingRepository incidentRecordingLogRepository, IIncidentRecordingQueryRepository incidentRecordingQueryRepository)
        {
            _notification = notification;
            _incidentRecordingLogRepository = incidentRecordingLogRepository;
            _incidentRecordingQueryRepository = incidentRecordingQueryRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var date = DateTime.Now;

            var sql = "CALL SP_INCIDENT_RECORDING_CREATE_LOG(@P_MAC_ADDRESS, @P_INCIDENT_TIME)";

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var incidentRecordingLog = await _incidentRecordingQueryRepository.GetByLogId(result.Id, cancellationToken);

            await _notification.SendIncidentRecordingCreatedLog(incidentRecordingLog);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
