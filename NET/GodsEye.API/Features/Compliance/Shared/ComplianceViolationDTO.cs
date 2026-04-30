using GodsEye.Shared.Enums;

namespace GodsEye.API.Features.Compliance.Shared
{
    public class ComplianceViolationDTO
    {
        public int LogId { get; set; }
        public int PolicyId { get; set; }
        public int PersonId { get; set; }
        public ComplianceViolationEnum Type { get; set; }
    }
}
