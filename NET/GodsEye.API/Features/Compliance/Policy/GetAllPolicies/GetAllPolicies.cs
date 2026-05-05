using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.Policy.GetAll
{
    public sealed record GetAllCompliancePoliciesQuery() : IRequest<IEnumerable<CompliancePolicyResponse>>;

    internal sealed record GetAllCompliancePoliciesHandler(IDapperContext context) : IRequestHandler<GetAllCompliancePoliciesQuery, IEnumerable<CompliancePolicyResponse>>
    {
        public async Task<IEnumerable<CompliancePolicyResponse>> Handle(GetAllCompliancePoliciesQuery request, CancellationToken cancellationToken)
        {
            return await GetAllComplianceQuery(cancellationToken);
        }

        public async Task<IEnumerable<CompliancePolicyResponse>> GetAllComplianceQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_GET_ALL()";

            return await context.QuerySqlAsync<CompliancePolicyResponse>(sql, cancellationToken);
        }
    }

    public class ComplianceController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllCompliancePoliciesQuery(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
