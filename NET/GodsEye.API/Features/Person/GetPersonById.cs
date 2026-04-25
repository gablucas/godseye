using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.AccessLevel;
using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetPersonByIdCommand(int id) : IRequest<PersonResponse>;

    internal sealed class GetPersonByIdHandler(IDapperContext context) : IRequestHandler<GetPersonByIdCommand, PersonResponse>
    {
        public async Task<PersonResponse> Handle(GetPersonByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetPersonByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<PersonResponse?> GetPersonByIdQuery(int personId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await context.QuerySingleSqlAsync<PersonResponse>(sql, parameters, cancellationToken);
        }
    }

    public class PersonEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/person/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetPersonByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
