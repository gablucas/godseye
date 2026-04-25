using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Godseye;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetMonitoringDataCommand() : IRequest<IEnumerable<MonitoringDataResponse>>;

    internal sealed record GetMonitoringDataHandler(IDapperContext context) : IRequestHandler<GetMonitoringDataCommand, IEnumerable<MonitoringDataResponse>>
    {
        public async Task<IEnumerable<MonitoringDataResponse>> Handle(GetMonitoringDataCommand request, CancellationToken cancellationToken)
        {
            return await GetMonitoringDataQuery(cancellationToken);
        }

        public async Task<IEnumerable<MonitoringDataResponse>> GetMonitoringDataQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_GODSEYE_GET_MONITORING_DATA()";

            return await context.QuerySqlAsync<MonitoringDataResponse>(sql, cancellationToken);
        }
    }

    public class GodsEyeController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/godseye", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetMonitoringDataCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
