using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.AccessLevel
{
    public sealed record GetAllComplianceCommand() : IRequest<IEnumerable<CompliancePolicyResponse>>;

    internal sealed record GetAllComplianceHandler(IDapperContext context) : IRequestHandler<GetAllComplianceCommand, IEnumerable<CompliancePolicyResponse>>
    {
        public async Task<IEnumerable<CompliancePolicyResponse>> Handle(GetAllComplianceCommand request, CancellationToken cancellationToken)
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
            var response = await mediator.Send(new GetAllComplianceCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
