using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace GodsEye.API.Features.Camera
{
    public sealed record GenerateIncidentLogRequest(string macAddress);

    public class GenerateIncidentLogValidator : AbstractValidator<GenerateIncidentLogRequest>
    {
        public GenerateIncidentLogValidator()
        {
            RuleFor(camera => camera.macAddress).NotEmpty();
        }
    }

    internal sealed record GenerateIncidentLogCommand(string macAddress) : IRequest<int>;

    internal sealed class GenerateIncidentLogMapper : Profile
    {
        public GenerateIncidentLogMapper()
        {
            CreateMap<GenerateIncidentLogRequest, GenerateIncidentLogCommand>();
        }
    }

    internal sealed class GenerateIncidentLogHandler(IDapperContext context, INotificationSignalR notification, ILogger<GenerateIncidentLogHandler> logger) : IRequestHandler<GenerateIncidentLogCommand, int>
    {
        public async Task<int> Handle(GenerateIncidentLogCommand request, CancellationToken cancellationToken)
        {
            var result = await GenerateIncidentLogWrite(request.macAddress, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            //var incidentRecordingLog = await GetLogById(result.Id, cancellationToken);

            //await notification.SendCreatedIncidentRecordingLog(incidentRecordingLog);

            return result.Id;
        }

        public async Task<ProcedureResult?> GenerateIncidentLogWrite(string macAddress, CancellationToken cancellationToken)
        {
            var date = DateTime.Now;

            var sql = "CALL SP_INCIDENT_RECORDING_CREATE_LOG(@P_MAC_ADDRESS, @P_INCIDENT_TIME)";

            var parameters = new
            {
                P_MAC_ADDRESS = macAddress,
                P_INCIDENT_TIME = date
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class IncidentRecordingController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/incident-recording", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] GenerateIncidentLogRequest request, 
            [FromServices] IValidator<GenerateIncidentLogRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<GenerateIncidentLogCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
