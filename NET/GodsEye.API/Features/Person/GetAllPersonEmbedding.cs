using GodsEye.API.DTO;
using GodsEye.API.Interfaces;

using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Person
{
    public sealed record GetAllPersonEmbeddingCommand() : IRequest<IEnumerable<PersonEmbeddingResponse>>;

    internal sealed record GetAllPersonEmbeddingHandler(IDapperContext context) : IRequestHandler<GetAllPersonEmbeddingCommand, IEnumerable<PersonEmbeddingResponse>>
    {
        public async Task<IEnumerable<PersonEmbeddingResponse>> Handle(GetAllPersonEmbeddingCommand request, CancellationToken cancellationToken)
        {
            return await GetAllPersonEmbeddingQuery(cancellationToken);
        }

        public async Task<IEnumerable<PersonEmbeddingResponse>> GetAllPersonEmbeddingQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_GET_ALL_EMBEDDING()";

            var parameters = new { };

            var persons = await context.QuerySqlAsync<PersonEmbeddingModel>(sql, parameters, cancellationToken);

            return await context.QuerySqlAsync<PersonEmbeddingResponse>(sql, cancellationToken);
        }
    }

    public class PersonEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/person/embedding", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllPersonEmbeddingCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
