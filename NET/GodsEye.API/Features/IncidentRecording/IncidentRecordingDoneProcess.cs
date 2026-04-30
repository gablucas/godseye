using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.API.Interfaces;

using GodsEye.API.ValueObjects;
using GodsEye.Shared.Response;
using GodsEye.Shared.Response.IncidentRecording;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace GodsEye.API.Features.IncidentRecording
{
    public sealed record UpdateIncidentRecordingLogRequest(int incidentId, List<IncidentRecordingPersonResponse> persons, string fileName);

    internal sealed record UpdateIncidentRecordingLogCommand(int incidentId, List<IncidentRecordingPersonResponse> persons, string fileName) : IRequest<int>;
    
    internal sealed class UpdateIncidentRecordingLogMapper : Profile
    {
        public UpdateIncidentRecordingLogMapper()
        {
            CreateMap<UpdateIncidentRecordingLogRequest, UpdateIncidentRecordingLogCommand>();
        }
    }

    internal sealed class UpdateIncidentRecordingLogHandler(Interfaces.IDapperContext context, IMapper mapper, IEmailService emailService, ILogger<UpdateIncidentRecordingLogHandler> logger) : IRequestHandler<UpdateIncidentRecordingLogCommand, int>
    {
        public async Task<int> Handle(UpdateIncidentRecordingLogCommand request, CancellationToken cancellationToken)
        {
            var personsVO = mapper.Map<List<IncidentRecordingPersonVO>>(request.persons);

            var result = await Update(request.incidentId, personsVO, request.fileName, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            var incident = await GetLogById(request.incidentId, cancellationToken);

            if (incident is null)
                throw new InvalidOperationException("Falha ao buscar dados do log no banco de dados");

            var html = await emailService.LoadTemplateAsync(
                "IncidentRecordingAlert.html",
                new Dictionary<string, string>
                {
                    ["pessoas"] = string.Join(", ", incident.Persons.Select(x => x.Name).ToList()),
                    ["date"] = incident?.ToString(),
                    ["videoUrl"] = $"http://localhost:8000/videos/{incident.FileName}"
                }
            );

            return 1;
        }

        private async Task<IncidentRecordingResponse?> GetLogById(int logId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_LOG_BY_ID(@P_ID)";

            var parameters = new
            {
                P_ID = logId
            };

            return await context.QuerySingleSqlAsync<IncidentRecordingResponse>(sql, parameters, cancellationToken);
        }

        public async Task<ProcedureResponse?> Update(int id, List<IncidentRecordingPersonVO> persons, string fileName, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_UPDATE_LOG(@P_ID, @P_PERSONS_IDS_JSON, @P_FILE_NAME)";

            var parameters = new
            {
                P_ID = id,
                P_PERSONS_IDS_JSON = JsonSerializer.Serialize(persons),
                P_FILE_NAME = fileName,
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class IncidentRecordingController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/incident-recording/process/done", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdateIncidentRecordingLogRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateIncidentRecordingLogCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
