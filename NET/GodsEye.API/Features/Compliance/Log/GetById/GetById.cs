using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.Log.GetById
{
    public sealed record GetComplianceByIdCommand(int id) : IRequest<CompliancePolicyResponse>;

    internal sealed class GetComplianceByIdHandler(IDapperContext context) : IRequestHandler<GetComplianceByIdCommand, CompliancePolicyResponse>
    {
        public async Task<CompliancePolicyResponse> Handle(GetComplianceByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetComplianceByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o compliance");

            return result;
        }

        public async Task<CompliancePolicyResponse?> GetComplianceByIdQuery(int complianceId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_GET_BY_ID(@P_COMPLIANCE_ID)";

            var parameters = new { P_COMPLIANCE_ID = complianceId };

            return await context.QuerySingleSqlAsync<CompliancePolicyResponse>(sql, parameters, cancellationToken);
        }
    }

    public class ComplianceController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetComplianceByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
