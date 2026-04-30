using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.Compliance
{
    public class ComplianceViolationResponse
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonPhoto { get; set; }
        public DateTime EnteredAt { get; set; }
        public DateTime ExitedAt { get; set; }
        public CompliancePolicyEnum PolicyType { get; set; }
        public ComplianceViolationEnum ViolationType { get; set; }
    }
}
