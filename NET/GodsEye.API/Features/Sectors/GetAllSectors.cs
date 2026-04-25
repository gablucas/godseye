using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Sector;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetAllSectorsCommand() : IRequest<IEnumerable<SectorResponse>>;

    internal sealed class GetAllSectorsHandler(IDapperContext context) : IRequestHandler<GetAllSectorsCommand, IEnumerable<SectorResponse>>
    {
        public async Task<IEnumerable<SectorResponse>> Handle(GetAllSectorsCommand request, CancellationToken cancellationToken)
        {
            return await GetAllSectorsQuery(cancellationToken);
        }

        public async Task<IEnumerable<SectorResponse>> GetAllSectorsQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_GET_ALL()";

            return await context.QuerySqlAsync<SectorResponse>(sql, cancellationToken);
        }
    }

    public class GetAllSectorsEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/sector", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAllSectorsCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
