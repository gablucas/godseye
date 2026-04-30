using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.AccessSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetAllAccessScheduleCommand() : IRequest<IEnumerable<AccessScheduleResponse>>;

    internal sealed record GetAllAccessScheduleHandler(IDapperContext context) : IRequestHandler<GetAllAccessScheduleCommand, IEnumerable<AccessScheduleResponse>>
    {
        public async Task<IEnumerable<AccessScheduleResponse>> Handle(GetAllAccessScheduleCommand request, CancellationToken cancellationToken)
        {
            return await GetAllAccessScheduleQuery(cancellationToken);
        }

        public async Task<IEnumerable<AccessScheduleResponse>> GetAllAccessScheduleQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_GET_ALL()";

            return await context.QuerySqlAsync<AccessScheduleResponse>(sql, cancellationToken);
        }
    }

    public class AccessScheduleEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/access-schedule", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllAccessScheduleCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
