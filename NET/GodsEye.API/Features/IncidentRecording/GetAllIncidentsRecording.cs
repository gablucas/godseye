using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.IncidentRecording;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetAllIncidentsRecordingCommand(int pageSize, int pageNumber) : IRequest<IEnumerable<IncidentRecordingResponse>>;

    internal sealed record GetAllIncidentsRecordingHandler(IDapperContext context) : IRequestHandler<GetAllIncidentsRecordingCommand, IEnumerable<IncidentRecordingResponse>>
    {
        public async Task<IEnumerable<IncidentRecordingResponse>> Handle(GetAllIncidentsRecordingCommand request, CancellationToken cancellationToken)
        {
            return await GetAllIncidentsRecordingQuery(request, cancellationToken);
        }

        public async Task<IEnumerable<IncidentRecordingResponse>> GetAllIncidentsRecordingQuery(GetAllIncidentsRecordingCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_ALL_LOGS(@P_PAGE_NUMBER, @P_PAGE_SIZE)";

            var parameters = new
            {
                P_PAGE_SIZE = request.pageSize,
                P_PAGE_NUMBER = request.pageNumber,
            };

            return await context.QuerySqlAsync<IncidentRecordingResponse>(sql, parameters, cancellationToken);
        }
    }

    public class IncidentRecordingController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/incident-recording", Handle);
        }

        private static async Task<IResult> Handle(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllIncidentsRecordingCommand(pageSize, pageNumber), cancellationToken);
            return Results.Ok(response);
        }
    }
}
