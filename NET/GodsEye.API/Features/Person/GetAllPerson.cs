using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetAllPersonCommand() : IRequest<IEnumerable<PersonResponse>>;

    internal sealed record GetAllPersonHandler(IDapperContext context) : IRequestHandler<GetAllPersonCommand, IEnumerable<PersonResponse>>
    {
        public async Task<IEnumerable<PersonResponse>> Handle(GetAllPersonCommand request, CancellationToken cancellationToken)
        {
            return await GetAllPersonQuery(cancellationToken);
        }

        public async Task<IEnumerable<PersonResponse>> GetAllPersonQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_GET_ALL()";

            return await context.QuerySqlAsync<PersonResponse>(sql, cancellationToken);
        }
    }

    public class PersonEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/person", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllPersonCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
