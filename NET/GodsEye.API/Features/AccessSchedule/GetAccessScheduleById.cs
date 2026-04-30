using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.AccessSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetAccessScheduleByIdCommand(int id) : IRequest<AccessScheduleResponse>;

    internal sealed class GetAccessScheduleByIdHandler(IDapperContext context) : IRequestHandler<GetAccessScheduleByIdCommand, AccessScheduleResponse>
    {
        public async Task<AccessScheduleResponse> Handle(GetAccessScheduleByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetAccessScheduleByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<AccessScheduleResponse?> GetAccessScheduleByIdQuery(int accessLevelId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_GET_BY_ID(@P_ACCESS_SCHEDULE_ID)";

            var parameters = new
            {
                P_ACCESS_SCHEDULE_ID = accessLevelId,
            };

            return await context.QuerySingleSqlAsync<AccessScheduleResponse>(sql, parameters, cancellationToken);
        }
    }

    public class AccessScheduleEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/access-schedule/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAccessScheduleByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
