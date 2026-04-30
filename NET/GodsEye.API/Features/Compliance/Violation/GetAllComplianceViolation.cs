using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.Violation
{
    public sealed record GetAllComplianceViolationCommand() : IRequest<IEnumerable<ComplianceViolationResponse>>;

    internal sealed class GetAllComplianceViolationHandler(IDapperContext context) : IRequestHandler<GetAllComplianceViolationCommand, IEnumerable<ComplianceViolationResponse>>
    {
        public async Task<IEnumerable<ComplianceViolationResponse>> Handle(GetAllComplianceViolationCommand request, CancellationToken cancellationToken)
        {
            return await GetAllComplianceViolationQuery(cancellationToken);
        }

        public async Task<IEnumerable<ComplianceViolationResponse>> GetAllComplianceViolationQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_VIOLATION_GET_ALL()";

            return await context.QuerySqlAsync<ComplianceViolationResponse>(sql, new { }, cancellationToken);
        }
    }

    public class ComplianceViolationEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/violation", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetAllComplianceViolationCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
