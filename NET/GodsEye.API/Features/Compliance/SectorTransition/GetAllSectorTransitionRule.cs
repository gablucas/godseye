using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public sealed record GetAllSectorTransitionRuleCommand() : IRequest<IEnumerable<SectorTransitionResponse>>;

    internal sealed class GetAllSectorTransitionRuleHandler(ISectorTransitionQuery sectorTransitionQuery) : IRequestHandler<GetAllSectorTransitionRuleCommand, IEnumerable<SectorTransitionResponse>>
    {
        public async Task<IEnumerable<SectorTransitionResponse>> Handle(GetAllSectorTransitionRuleCommand request, CancellationToken cancellationToken)
        {
            var result = await sectorTransitionQuery.GetAll(cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }
    }

    public class GetAllSectorTransitionRuleEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/sector-transition", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAllSectorTransitionRuleCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
