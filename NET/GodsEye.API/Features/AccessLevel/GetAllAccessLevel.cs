using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.AccessLevel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetAllAccessLevelCommand() : IRequest<IEnumerable<AccessLevelResponse>>;

    internal sealed record GetAllAccessLevelHandler(IDapperContext context) : IRequestHandler<GetAllAccessLevelCommand, IEnumerable<AccessLevelResponse>>
    {
        public async Task<IEnumerable<AccessLevelResponse>> Handle(GetAllAccessLevelCommand request, CancellationToken cancellationToken)
        {
            return await GetAllAccessLevelQuery(cancellationToken);
        }

        public async Task<IEnumerable<AccessLevelResponse>> GetAllAccessLevelQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_GET_ALL()";

            return await context.QuerySqlAsync<AccessLevelResponse>(sql, cancellationToken);
        }
    }

    public class AccessLevelEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/access-level", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllAccessLevelCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
