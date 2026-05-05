using GodsEye.API.Features.Compliance.Shared.Query;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.SectorTransition.GetById
{
    public sealed record GetSectorTransitionByIdCommand(int id) : IRequest<SectorTransitionResponse>;

    internal sealed class GetSectorTransitionByIdHandler(ISectorTransitionQuery sectorTransitionQuery) : IRequestHandler<GetSectorTransitionByIdCommand, SectorTransitionResponse>
    {
        public async Task<SectorTransitionResponse> Handle(GetSectorTransitionByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await sectorTransitionQuery.GetRuleById(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }
    }

    public class SectroTransitionController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/sector-transition/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetSectorTransitionByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
