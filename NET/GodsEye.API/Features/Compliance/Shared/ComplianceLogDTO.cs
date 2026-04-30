namespace GodsEye.API.Features.Compliance.Shared
{
    public class ComplianceLogDTO
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int SectorId { get; set; }
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
    }
}
