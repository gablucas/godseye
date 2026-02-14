using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.ValueObjects;
using MediatR;
using System.Data;
using System.Text.Json;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public class UpdateIncidentRecordingLogHandler : IRequestHandler<UpdateIncidentRecordingLogRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IApplicationDbContext _context;

        public UpdateIncidentRecordingLogHandler(IMapper mapper, IEmailService emailService, IApplicationDbContext context)
        {
            _mapper = mapper;
            _emailService = emailService;
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var personsVO = _mapper.Map<List<IncidentRecordingPersonVO>>(request.persons);

            var result = await Update(request.incidentId, personsVO, request.fileName, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var incident = await GetLogById(request.incidentId, cancellationToken);

            var html = await _emailService.LoadTemplateAsync(
                "IncidentRecordingAlert.html",
                new Dictionary<string, string>
                {
                    ["pessoas"] = string.Join(", ", incident.Persons.Select(x => x.Name).ToList()),
                    ["date"] = incident.IncidentTime.ToString(),
                    ["videoUrl"] = $"http://localhost:8000/videos/{incident.FileName}"
                }
            );

            //await _emailService.SendAsync(["gabriel.pegoretti96@gmail.com", "contato@agse.com.br", "thiago@nbwdigital.com.br"], "Teste", html);


            //await _emailService.SendAsync("gabriel.pegoretti96@gmail.com");

            return ApiResponse<ProcedureResult>.Ok(result);
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

        public async Task<ProcedureResult?> Update(int id, List<IncidentRecordingPersonVO> persons, string fileName, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_UPDATE_LOG(@P_ID, @P_PERSONS_IDS_JSON, @P_FILE_NAME)";

            var parameters = new
            {
                P_ID = id,
                P_PERSONS_IDS_JSON = persons,
                P_FILE_NAME = fileName,
            };

            return await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }
}
