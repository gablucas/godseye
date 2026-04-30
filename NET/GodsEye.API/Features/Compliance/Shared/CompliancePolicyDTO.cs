using GodsEye.Shared.Enums;

namespace GodsEye.API.Features.Compliance.Shared
{
    public class CompliancePolicyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComplianceRuleEnum Rule { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
