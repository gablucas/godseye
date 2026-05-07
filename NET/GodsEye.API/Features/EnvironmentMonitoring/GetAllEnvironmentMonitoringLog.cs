using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace GodsEye.API.Features.EnvironmentMonitoring
{
    public sealed record GetAllEnvironmentMonitoringLogsRequest(int pageNumber, int pageSize) : IRequest<IEnumerable<EnvironmentMonitoringResponse>>;

    internal sealed class GetAllEnvironmentMonitoringLogsHandler(IDapperContext context) : IRequestHandler<GetAllEnvironmentMonitoringLogsRequest, IEnumerable<EnvironmentMonitoringResponse>>
    {

        public async Task<IEnumerable<EnvironmentMonitoringResponse>> Handle(GetAllEnvironmentMonitoringLogsRequest request, CancellationToken cancellationToken)
        {
            var result = await GetAll(request.pageNumber, request.pageSize, CancellationToken.None);

            if (result is null)
                throw new InvalidOperationException("Falha ao registrar log no banco de dados");

            return result;
        }

        public async Task<IEnumerable<EnvironmentMonitoringResponse>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_ALL(@P_PAGE_NUMBER, @P_PAGE_SIZE)";

            var parameters = new
            {
                P_PAGE_NUMBER = pageNumber,
                P_PAGE_SIZE = pageSize,
            };

            return await context.QuerySqlAsync<EnvironmentMonitoringResponse>(query, parameters, cancellationToken);
        }
    }

    public class GetAllEnvironmentMonitoringLogsEndpoint : IEndpoint 
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/environment-monitoring", Handle);
        }

        private static async Task<IResult> Handle(
            [AsParameters] GetAllEnvironmentMonitoringLogsRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    }
}
