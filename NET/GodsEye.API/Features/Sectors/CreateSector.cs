using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;

using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Sectors
{
    public sealed record CreateSectorRequest(string Name, int? ParentId, IEnumerable<int> NotificationGroups);

    public class CreateSectorValidator : AbstractValidator<CreateSectorRequest>
    {
        public CreateSectorValidator()
        {
            RuleFor(sector => sector.Name).NotEmpty();
            RuleFor(sector => sector.NotificationGroups)
                .NotEmpty()
                .ForEach(rule => rule.GreaterThan(0));
        }
    }

    internal sealed record CreateSectorCommand(string Name, int? ParentId, IEnumerable<int> NotificationGroups) : IRequest<int>;

    internal sealed class CreateSectorMapper : Profile
    {
        public CreateSectorMapper()
        {
            CreateMap<CreateSectorRequest, CreateSectorCommand>();
        }
    }

    internal sealed class CreateSectorHandler(IDapperContext context, ILogger<CreateSectorHandler> logger) : IRequestHandler<CreateSectorCommand, int>
    {
        public async Task<int> Handle(CreateSectorCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateSectorWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        private async Task<ProcedureResponse?> CreateSectorWrite(CreateSectorCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_CREATE(@P_NAME, @P_PARENT_ID, @P_NOTIFICATION_GROUP_JSON)";

            var parameters = new
            {
                P_NAME = request.Name,
                P_PARENT_ID = request.ParentId,
                P_NOTIFICATION_GROUP_JSON = JsonSerializer.Serialize(request.NotificationGroups)
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class CreateSectorEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/sector", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateSectorRequest request,
            [FromServices] IValidator<CreateSectorRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateSectorCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
