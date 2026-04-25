using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Sector;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetSectorByIdCommand(int id) : IRequest<SectorResponse>;

    internal sealed class GetSectorByIdHandler(IDapperContext context) : IRequestHandler<GetSectorByIdCommand, SectorResponse>
    {
        public async Task<SectorResponse> Handle(GetSectorByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetSectorByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<SectorResponse?> GetSectorByIdQuery(int sectorId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_GET_BY_ID(@P_SECTOR_ID)";

            var parameters = new
            {
                P_SECTOR_ID = sectorId,
            };

            return await context.QuerySingleSqlAsync<SectorResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetSectorByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/sector/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetSectorByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
