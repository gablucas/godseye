using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.CreateIncidentRecordingLog
{
    public class CreateIncidentRecordingLogHandler : IRequestHandler<CreateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly INotificationSignalR _notification;
        private readonly IApplicationDbContext _context;

        public CreateIncidentRecordingLogHandler(INotificationSignalR notification, IApplicationDbContext context)
        {
            _notification = notification;
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var result = await CreateLog(request.macAddress, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var incidentRecordingLog = await GetLogById(result.Id, cancellationToken);

            await _notification.SendCreatedIncidentRecordingLog(incidentRecordingLog);

            return ApiResponse<ProcedureResult>.Ok(result);
        }

        private async Task<ProcedureResult?> CreateLog(string macAddress, CancellationToken cancellationToken)
        {
            var date = DateTime.Now;

            var sql = "CALL SP_INCIDENT_RECORDING_CREATE_LOG(@P_MAC_ADDRESS, @P_INCIDENT_TIME)";

            var parameters = new
            {
                P_MAC_ADDRESS = macAddress,
                P_INCIDENT_TIME = date
            };

            return await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }

        private async Task<IncidentRecordingModel?> GetLogById(int logId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_LOG_BY_ID(@P_ID)";

            var parameters = new
            {
                P_ID = logId
            };

            return await _context.QuerySingleSqlAsync<IncidentRecordingModel>(sql, parameters, cancellationToken);
        }
    }
}
