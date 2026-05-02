using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.Compliance
{
    public class ComplianceViolationResponse
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonPhoto { get; set; }
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public DateTime? EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public CompliancePolicyEnum PolicyType { get; set; }
        public ComplianceViolationEnum ViolationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
