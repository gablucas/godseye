using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Compliance.SectorTransition.CreateRule
{
    public sealed record CreateRuleSectorTransitionRequest(int policyId, string policyName, List<SectorTransitionRuleDTO> rules);

    public sealed record SectorTransitionRuleDTO(int orderIndex, int? minTime, int? maxTime, int? sectorId);

    public class CreateRuleSectorTransitionValidator : AbstractValidator<CreateRuleSectorTransitionRequest>
    {
        public CreateRuleSectorTransitionValidator()
        {
            //RuleFor(camera => camera.policyId).NotEmpty();
            RuleFor(camera => camera.policyName).NotEmpty();
            RuleFor(camera => camera.rules).NotEmpty();
        }
    }

    internal sealed record CreateRuleSectorTransitionCommand(int policyId, string policyName, List<SectorTransitionRuleDTO> rules) : IRequest<int>;

    internal sealed class CreateRuleSectorTransitionMapper : Profile
    {
        public CreateRuleSectorTransitionMapper()
        {
            CreateMap<CreateRuleSectorTransitionRequest, CreateRuleSectorTransitionCommand>();
        }
    }

    internal sealed class CreateRuleSectorTransitionHandler(IDapperContext context, ILogger<CreateRuleSectorTransitionHandler> logger) : IRequestHandler<CreateRuleSectorTransitionCommand, int>
    {
        public async Task<int> Handle(CreateRuleSectorTransitionCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateRuleSectorTransitionWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> CreateRuleSectorTransitionWrite(CreateRuleSectorTransitionCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_RULE_SECTOR_TRANSITION_CREATE(@P_POLICY_NAME, @P_RULE_JSON)";

            var parameters = new
            {
                P_POLICY_NAME = request.policyName,
                P_RULE_JSON = JsonSerializer.Serialize(request.rules)
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class ComplianceController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/compliance/rule/sector-transitions", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateRuleSectorTransitionRequest request, 
            [FromServices] IValidator<CreateRuleSectorTransitionRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateRuleSectorTransitionCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
