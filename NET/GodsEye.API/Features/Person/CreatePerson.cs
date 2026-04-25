using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreatePersonRequest(string name, int sectorId, int accessLevelId);

    public class CreatePersonValidator : AbstractValidator<CreatePersonRequest>
    {
        public CreatePersonValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.sectorId).NotEmpty();
            RuleFor(camera => camera.accessLevelId).NotEmpty();
        }
    }

    internal sealed record CreatePersonCommand(string name, int sectorId, int accessLevelId) : IRequest<int>;

    internal sealed class CreatePersonMapper : Profile
    {
        public CreatePersonMapper()
        {
            CreateMap<CreatePersonRequest, CreatePersonCommand>();
        }
    }

    internal sealed class CreatePersonHandler(IDapperContext context, ILogger<CreatePersonHandler> logger, INotificationSignalR notification) : IRequestHandler<CreatePersonCommand, int>
    {
        public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var result = await CreatePersonWrite(request, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            var createdPerson = await GetById(result.Id, cancellationToken);

            //if (createdPerson is not null)
            //    await notification.SendCreatedPerson(createdPerson);

            return result.Id;
        }

        public async Task<ProcedureResult?> CreatePersonWrite(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_CREATE(@P_NAME, @P_MAIN_SECTOR_ID, @P_ACCESS_LEVEL_ID)";

            var parameters = new
            {
                P_NAME = request.name,
                P_MAIN_SECTOR_ID = request.sectorId,
                P_ACCESS_LEVEL_ID = request.accessLevelId
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }

        public async Task<PersonResponse?> GetById(int personId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await context.QuerySingleSqlAsync<PersonResponse>(query, parameters, cancellationToken);
        }
    }

    public class CreatePersonEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/person", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreatePersonRequest request, 
            [FromServices] IValidator<CreatePersonRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreatePersonCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
