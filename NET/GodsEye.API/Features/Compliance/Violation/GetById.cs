using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.Violation
{
    public sealed record GetComplianceViolationByIdCommand(int id) : IRequest<ComplianceViolationResponse>;

    internal sealed class GetComplianceViolationByIdHandler(IDapperContext context) : IRequestHandler<GetComplianceViolationByIdCommand, ComplianceViolationResponse>
    {
        public async Task<ComplianceViolationResponse> Handle(GetComplianceViolationByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetComplianceViolationByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o compliance");

            return result;
        }

        public async Task<ComplianceViolationResponse?> GetComplianceViolationByIdQuery(int complianceViolationId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_VIOLATION_GET_BY_ID(@P_COMPLIANCE_VIOLATION_ID)";

            var parameters = new { P_COMPLIANCE_VIOLATION_ID = complianceViolationId };

            return await context.QuerySingleSqlAsync<ComplianceViolationResponse>(sql, parameters, cancellationToken);
        }
    }

    public class ComplianceViolationController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/violation/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetComplianceViolationByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
