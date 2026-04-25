using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.Queries;
using MediatR;

namespace GodsEye.Application.UseCases.Compliance.Queries
{
    public sealed record GetComplianceByIdRequest(int complianceId) : IRequest<CompliancePolicyDTO>;

    public class GetComplianceByHandler : IRequestHandler<GetComplianceByIdRequest, CompliancePolicyDTO>
    {
        private readonly IComplianceQuerie _complianceQuery;

        public GetComplianceByHandler(IComplianceQuerie complianceQuery)
        {
            _complianceQuery = complianceQuery;
        }

        public async Task<CompliancePolicyDTO> Handle(GetComplianceByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _complianceQuery.GeById(request.complianceId, cancellationToken);

            if (result == null)
                throw new KeyNotFoundException($"Compliance policy {request.complianceId} not found");

            return result;
        }
    }
}
