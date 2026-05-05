using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Compliance.Violation
{
    public sealed record GetAllComplianceViolationCommand(int pageNumber, int pageSize) : IRequest<IEnumerable<ComplianceViolationResponse>>;

    internal sealed class GetAllComplianceViolationHandler(IComplianceViolationQuery complianceViolationQuery) : IRequestHandler<GetAllComplianceViolationCommand, IEnumerable<ComplianceViolationResponse>>
    {
        public async Task<IEnumerable<ComplianceViolationResponse>> Handle(GetAllComplianceViolationCommand request, CancellationToken cancellationToken)
        {
            return await complianceViolationQuery.GetAllComplianceViolationQuery(request.pageNumber, request.pageSize, cancellationToken);
        }
    }

    public class ComplianceViolationEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/violation", Handle);
        }

        private static async Task<IResult> Handle(
            [AsParameters] GetAllComplianceViolationCommand request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    }
}
