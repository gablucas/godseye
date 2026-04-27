using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Godseye;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.GodsEye
{
    public sealed record GetMonitoringDataCommand() : IRequest<MonitoringDataResponse>;

    internal sealed record GetMonitoringDataHandler(IDapperContext context) : IRequestHandler<GetMonitoringDataCommand, MonitoringDataResponse>
    {
        public async Task<MonitoringDataResponse> Handle(GetMonitoringDataCommand request, CancellationToken cancellationToken)
        {
            var data = await GetMonitoringDataQuery(cancellationToken);

            if (data == null)
                throw new InvalidOperationException("Não foi possível buscar os dados das cameras para o monitoramento");

            return data;
        }

        public async Task<MonitoringDataResponse?> GetMonitoringDataQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_GODSEYE_GET_MONITORING_DATA()";

            return await context.QuerySingleSqlAsync<MonitoringDataResponse>(sql, new { }, cancellationToken);
        }
    }

    public class GodsEyeEndpoint : IEndpoint
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
