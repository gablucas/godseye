using GodsEye.Shared.Enums;

namespace GodsEye.API.Features.Compliance.Shared
{
    public interface IComplianceStrategy
    {
        CompliancePolicyEnum RuleType { get; }
        Task Apply(int policyLogId, int personId, int sectorId, CompliancePolicyDTO policy, CancellationToken cancellationToken);
    }
}
