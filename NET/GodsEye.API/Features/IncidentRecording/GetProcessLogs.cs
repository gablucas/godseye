using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.IncidentRecording;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.IncidentRecording
{
    public sealed record GetProcessLogsCommand() : IRequest<IEnumerable<IncidentRecordingLogResponse>>;

    internal sealed record GetProcessLogsHandler(IDapperContext context) : IRequestHandler<GetProcessLogsCommand, IEnumerable<IncidentRecordingLogResponse>>
    {
        public async Task<IEnumerable<IncidentRecordingLogResponse>> Handle(GetProcessLogsCommand request, CancellationToken cancellationToken)
        {
            return await GetProcessLogsQuery(request, cancellationToken);
        }

        public async Task<IEnumerable<IncidentRecordingLogResponse>> GetProcessLogsQuery(GetProcessLogsCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_TO_PROCESS()";

            return await context.QuerySqlAsync<IncidentRecordingLogResponse>(sql, cancellationToken);
        }
    }

    public class IncidentRecordingEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/incident-recording/process", Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetProcessLogsCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
