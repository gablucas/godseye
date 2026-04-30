using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;

using GodsEye.Shared.Enums;
using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreateAccessScheduleRequest(int id, string name, bool isActive, List<scheduleRuleDTO> rules);
    public sealed record scheduleRuleDTO(int id, WeekDayEnum weekDay, TimeSpan? startTime, TimeSpan? endTime);

    public class CreateAccessScheduleValidator : AbstractValidator<CreateAccessScheduleRequest>
    {
        public CreateAccessScheduleValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.isActive).NotEmpty();
            RuleFor(camera => camera.rules).NotEmpty();
        }
    }

    internal sealed record CreateAccessScheduleCommand(int id, string name, bool isActive, List<scheduleRuleDTO> rules) : IRequest<int>;

    internal sealed class CreateAccessScheduleMapper : Profile
    {
        public CreateAccessScheduleMapper()
        {
            CreateMap<CreateAccessScheduleRequest, CreateAccessScheduleCommand>();
        }
    }

    internal sealed class CreateAccessScheduleHandler(IDapperContext context, ILogger<CreateAccessScheduleHandler> logger) : IRequestHandler<CreateAccessScheduleCommand, int>
    {
        public async Task<int> Handle(CreateAccessScheduleCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateAccessScheduleWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> CreateAccessScheduleWrite(CreateAccessScheduleCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_IS_ACTIVE, @P_RULES_JSON)";

            var parameters = new
            {
                P_ID = request.id,
                P_NAME = request.name,
                P_IS_ACTIVE = request.isActive,
                P_RULES_JSON = request.rules
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class AccessScheduleEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/access-schedule", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateAccessScheduleRequest request, 
            [FromServices] IValidator<CreateAccessScheduleRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateAccessScheduleCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
