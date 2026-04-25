using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.Queries;
using MediatR;

namespace GodsEye.Application.UseCases.Compliance.Queries
{
    public sealed record GetAllComplianceRequest() : IRequest<IEnumerable<CompliancePolicyDTO>>;

    public class GetAllComplianceHandler : IRequestHandler<GetAllComplianceRequest, IEnumerable<CompliancePolicyDTO>>
    {
        private readonly IComplianceQuerie _complianceQuerie;

        public GetAllComplianceHandler(IComplianceQuerie complianceQuerie)
        {
            _complianceQuerie = complianceQuerie;
        }

        public async Task<IEnumerable<CompliancePolicyDTO>> Handle(GetAllComplianceRequest request, CancellationToken cancellationToken)
        {
            return await _complianceQuerie.GetAll(cancellationToken);
        }
    }
}
