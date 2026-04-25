using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.AccessLevel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetAccessLevelByIdCommand(int id) : IRequest<AccessLevelResponse>;

    internal sealed class GetAccessLevelByIdHandler(IDapperContext context) : IRequestHandler<GetAccessLevelByIdCommand, AccessLevelResponse>
    {
        public async Task<AccessLevelResponse> Handle(GetAccessLevelByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetAccessLevelByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<AccessLevelResponse?> GetAccessLevelByIdQuery(int accessLevelId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_GET_BY_ID(@P_ACCESS_LEVEL_ID)";

            var parameters = new
            {
                P_ACCESS_LEVEL_ID = accessLevelId
            };

            return await context.QuerySingleSqlAsync<AccessLevelResponse>(sql, parameters, cancellationToken);
        }
    }

    public class AccessLevelEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/access-level/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAccessLevelByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
