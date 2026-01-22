using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Domain.ValueObjects;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public class UpdateIncidentRecordingLogHandler : IRequestHandler<UpdateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IIncidentRecordingLogRepository _incidentRecordingLogRepository;
        private readonly IIncidentRecordingQueryRepository _incidentRecordingQueryRepository;

        public UpdateIncidentRecordingLogHandler(IMapper mapper, IEmailService emailService, IIncidentRecordingLogRepository incidentRecordingLogRepository, IIncidentRecordingQueryRepository incidentRecordingQueryRepository)
        {
            _mapper = mapper;
            _emailService = emailService;
            _incidentRecordingLogRepository = incidentRecordingLogRepository;
            _incidentRecordingQueryRepository = incidentRecordingQueryRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var personsVO = _mapper.Map<List<IncidentRecordingPersonVO>>(request.persons);

            var result = await _incidentRecordingLogRepository.Update(request.incidentId, personsVO, request.fileName);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var incident = await _incidentRecordingQueryRepository.GetByLogId(request.incidentId, cancellationToken);

            var html = await _emailService.LoadTemplateAsync(
                "IncidentRecordingAlert.html",
                new Dictionary<string, string>
                {
                    ["pessoas"] = string.Join(", ", incident.Persons.Select(x => x.Name).ToList()),
                    ["date"] = incident.IncidentTime.ToString(),
                    ["videoUrl"] = $"http://localhost:8000/videos/{incident.FileName}"
                }
            );

            await _emailService.SendAsync(["gabriel.pegoretti96@gmail.com"], "Teste", html);


            //await _emailService.SendAsync("gabriel.pegoretti96@gmail.com");

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
