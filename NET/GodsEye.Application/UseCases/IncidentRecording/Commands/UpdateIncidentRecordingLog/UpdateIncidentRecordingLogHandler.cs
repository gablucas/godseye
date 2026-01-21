using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Domain.ValueObjects;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public class UpdateIncidentRecordingLogHandler : IRequestHandler<UpdateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly IIncidentRecordingLogRepository _incidentRecordingLogRepository;

        public UpdateIncidentRecordingLogHandler(IMapper mapper, IIncidentRecordingLogRepository incidentRecordingLogRepository)
        {
            _mapper = mapper;
            _incidentRecordingLogRepository = incidentRecordingLogRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var personsVO = _mapper.Map<List<IncidentRecordingPersonVO>>(request.persons);

            var result = await _incidentRecordingLogRepository.Update(request.incidentId, personsVO, request.videoPath);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
