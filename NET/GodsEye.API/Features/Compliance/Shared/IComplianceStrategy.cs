namespace GodsEye.API.Features.Compliance.Shared
{
    public interface IComplianceStrategy
    {
        ComplianceRuleEnum RuleType { get; }
        Task Apply(int policyLogId, int personId, int sectorId, CompliancePolicyDTO policy, CancellationToken cancellationToken);
    }
}
