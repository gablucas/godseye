using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreateOrUpdateAccessLevelRequest(int id, string name, List<SectorAccessLevelInput> sectors, int? accessScheduleId);
    public sealed record SectorAccessLevelInput(int sectorId, AccessLevelSectorRuleEnum ruleType);

    public class CreateOrUpdateAccessLevelValidator : AbstractValidator<CreateOrUpdateAccessLevelRequest>
    {
        public CreateOrUpdateAccessLevelValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.sectors).NotEmpty();
        }
    }

    internal sealed record CreateOrUpdateAccessLevelCommand(int id, string name, List<SectorAccessLevelInput> sectors, int? accessScheduleId) : IRequest<int>;

    internal sealed class CreateOrUpdateAccessLevelMapper : Profile
    {
        public CreateOrUpdateAccessLevelMapper()
        {
            CreateMap<CreateOrUpdateAccessLevelRequest, CreateOrUpdateAccessLevelCommand>();
        }
    }

    internal sealed class CreateOrUpdateAccessLevelHandler(IDapperContext context, ILogger<CreateOrUpdateAccessLevelHandler> logger) : IRequestHandler<CreateOrUpdateAccessLevelCommand, int>
    {
        public async Task<int> Handle(CreateOrUpdateAccessLevelCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateOrUpdateAccessLevelWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResult?> CreateOrUpdateAccessLevelWrite(CreateOrUpdateAccessLevelCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_SECTORS_JSON, @P_ACCESS_SCHEDULE_ID)";

            var parameters = new
            {
                P_ID = request.id,
                P_NAME = request.name,
                P_SECTORS_JSON = JsonSerializer.Serialize(request.sectors),
                P_ACCESS_SCHEDULE_ID = request.accessScheduleId
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class AccessLevelEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/access-level", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateOrUpdateAccessLevelRequest request, 
            [FromServices] IValidator<CreateOrUpdateAccessLevelRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateOrUpdateAccessLevelCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
