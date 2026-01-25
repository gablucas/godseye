namespace GodsEye.Domain.Entities
{
    public class DwellTimeMonitoringEntity : BaseEntity
    {
        public int CameraId { get; set; }
        public int PersonId { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
