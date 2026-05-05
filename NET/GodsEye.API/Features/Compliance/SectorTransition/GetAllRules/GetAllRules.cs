using GodsEye.API.Features.Compliance.Shared.Query;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.SectorTransition.GetAll
{
    public sealed record GetAllSectorTransitionRulesCommand() : IRequest<IEnumerable<SectorTransitionResponse>>;

    internal sealed class GetAllSectorTransitionRulesHandler(ISectorTransitionQuery sectorTransitionQuery) : IRequestHandler<GetAllSectorTransitionRulesCommand, IEnumerable<SectorTransitionResponse>>
    {
        public async Task<IEnumerable<SectorTransitionResponse>> Handle(GetAllSectorTransitionRulesCommand request, CancellationToken cancellationToken)
        {
            var result = await sectorTransitionQuery.GetAll(cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }
    }

    public class SectorTransitionController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/sector-transition", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAllSectorTransitionRulesCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
