using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public class UpdateIncidentRecordingLogHandler : IRequestHandler<UpdateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IIncidentRecordingLogRepository _incidentRecordingLogRepository;

        public UpdateIncidentRecordingLogHandler(IIncidentRecordingLogRepository incidentRecordingLogRepository)
        {
            _incidentRecordingLogRepository = incidentRecordingLogRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentRecordingLogRepository.Update(request.id, request.personId);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
