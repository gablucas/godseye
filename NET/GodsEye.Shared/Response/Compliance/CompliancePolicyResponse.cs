using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.Compliance
{
    public class CompliancePolicyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComplianceRuleEnum Rule { get; set; }
    }
}
