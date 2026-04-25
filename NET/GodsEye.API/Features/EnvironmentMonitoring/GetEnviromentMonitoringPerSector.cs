using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.EnvironmentMonitoring
{

    public sealed record GetEnviromentMonitoringPerSectorRequest() : IRequest<IEnumerable<GetEnviromentMonitoringPerSectorResponse>>;

    internal sealed class GetEnvironmentMonitoringPerSectorHandler(IDapperContext context) : IRequestHandler<GetEnviromentMonitoringPerSectorRequest, IEnumerable<GetEnviromentMonitoringPerSectorResponse>>
    {
        public async Task<IEnumerable<GetEnviromentMonitoringPerSectorResponse>> Handle(GetEnviromentMonitoringPerSectorRequest request, CancellationToken cancellationToken)
        {
            var result = await GetEnvironmentMonitoringPerSector(cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return result;
        }

        public async Task<IEnumerable<GetEnviromentMonitoringPerSectorResponse>> GetEnvironmentMonitoringPerSector(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_GET_SECTORS()";
            return await context.QuerySqlAsync<GetEnviromentMonitoringPerSectorResponse>(sql, cancellationToken);
        }
    }

    public class GetEnviromentMonitoringPerSectorEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/environmentmonitoring/sectors", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetEnviromentMonitoringPerSectorRequest(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
