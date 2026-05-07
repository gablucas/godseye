using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.EnvironmentMonitoring
{
    public sealed record GetLastEnvironmentMonitoringLogPerPersonRequest() : IRequest<IEnumerable<EnvironmentMonitoringResponse>>;

    internal sealed class GetLastEnvironmentMonitoringLogPerPersonHandler(IDapperContext context) : IRequestHandler<GetLastEnvironmentMonitoringLogPerPersonRequest, IEnumerable<EnvironmentMonitoringResponse>>
    {

        public async Task<IEnumerable<EnvironmentMonitoringResponse>> Handle(GetLastEnvironmentMonitoringLogPerPersonRequest request, CancellationToken cancellationToken)
        {
            var result = await GetLastRegisterPerPerson(cancellationToken);

            return result;
        }

        protected async Task<IEnumerable<EnvironmentMonitoringResponse>> GetLastRegisterPerPerson(CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_LAST_REGISTER_PER_PERSON()";

            return await context.QuerySqlAsync<EnvironmentMonitoringResponse>(query, cancellationToken);
        }
    }

    public class GetLastEnvironmentMonitoringLogPerPersonEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/environment-monitoring/log/last-per-person", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetLastEnvironmentMonitoringLogPerPersonRequest(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
