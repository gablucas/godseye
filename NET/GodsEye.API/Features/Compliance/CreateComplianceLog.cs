using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace GodsEye.API.Features.Compliance
{

    public sealed record CreateComplianceLogRequest(int PersonId, int SectorId, DateTime IdentifiedAt);

    public class CreateComplianceLogValidator : AbstractValidator<CreateComplianceLogRequest>
    {
        public CreateComplianceLogValidator()
        {
            RuleFor(log => log.PersonId).NotEmpty();
            RuleFor(log => log.SectorId).NotEmpty();
            RuleFor(log => log.IdentifiedAt).NotEmpty();
        }
    }

    internal sealed record CreateComplianceLogCommand(int personId, int sectorId, DateTime identifiedAt) : IRequest<int>;

    internal sealed class CreateComplianceLogMapper : Profile
    {
        public CreateComplianceLogMapper()
        {
            CreateMap<CreateComplianceLogRequest, CreateComplianceLogCommand>();
        }
    }

    internal sealed class CreateComplianceLogHandler(IDapperContext context, ILogger<CreateComplianceLogHandler> logger) : IRequestHandler<CreateComplianceLogCommand, int>
    {
        public async Task<int> Handle(CreateComplianceLogCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateComplianceLogWrite(request, cancellationToken);

            if (result.Id == 0)
            {
                string message = "Houve um erro ao inserir o log do compliance";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }


        private async Task<ProcedureResult> CreateComplianceLogWrite(CreateComplianceLogCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_LOG_CREATE(@P_PERSON_ID, @P_SECTOR_ID, @P_IDENTIFIED_AT)";

            var parameters = new
            {
                P_PERSON_ID = request.personId,
                P_SECTOR_ID = request.sectorId,
                P_IDENTIFIED_AT = request.identifiedAt
            };

            var result = await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return result;
        }
    }

    public class CraeteComplianceLogEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/compliance/log", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateComplianceLogRequest request, 
            [FromServices] IValidator<CreateComplianceLogRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateComplianceLogCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
