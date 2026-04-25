using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.EnvironmentMonitoring
{
    public sealed record GetEnvironmentMonitoringLogsByPersonIdRequest(int PersonId) : IRequest<EnvironmentMonitoringPersonResponse>;

    internal sealed class GetEnvironmentMonitoringLogsByPersonIdHandler(IDapperContext context) : IRequestHandler<GetEnvironmentMonitoringLogsByPersonIdRequest, EnvironmentMonitoringPersonResponse>
    {
        public async Task<EnvironmentMonitoringPersonResponse> Handle(GetEnvironmentMonitoringLogsByPersonIdRequest request, CancellationToken cancellationToken)
        {
            var result = await GetEnvironmentMonitoringLogsByPersonIdQuerie(request.PersonId, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return result;
        }

        public async Task<EnvironmentMonitoringPersonResponse?> GetEnvironmentMonitoringLogsByPersonIdQuerie(int personId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_PERSON_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await context.QuerySingleSqlAsync<EnvironmentMonitoringPersonResponse>(query, parameters, cancellationToken);
        }
    }

    public class GetEnvironmentMonitoringLogsByPersonIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/environmentmonitoring/person", Handle);
        }

        private static async Task<IResult> Handle(
            [AsParameters] GetEnvironmentMonitoringLogsByPersonIdRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    }
}
