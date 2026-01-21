using GodsEye.Domain.Enums;

namespace GodsEye.Domain.Entities
{
    public class IncidentRecordingEntity : BaseEntity
    {
        public int CameraId { get; set; }
        public DateTime IncidentTime { get; set; }
        public IncidentStatusEnum Status { get; set; }
        public string VideoPath { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
