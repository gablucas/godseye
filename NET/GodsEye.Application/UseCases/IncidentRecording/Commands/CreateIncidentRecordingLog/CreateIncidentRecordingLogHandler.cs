using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.CreateIncidentRecordingLog
{
    public class CreateIncidentRecordingLogHandler : IRequestHandler<CreateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IIncidentRecordingLogRepository _incidentRecordingLogRepository;

        public CreateIncidentRecordingLogHandler(IIncidentRecordingLogRepository incidentRecordingLogRepository)
        {
            _incidentRecordingLogRepository = incidentRecordingLogRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentRecordingLogRepository.Create(request.cameraId, request.incidentTime);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
