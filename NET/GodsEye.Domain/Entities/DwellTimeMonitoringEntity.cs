namespace GodsEye.Domain.Entities
{
    public class DwellTimeMonitoringEntity : BaseEntity
    {
        public int CameraId { get; set; }
        public int PersonId { get; set; }
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
