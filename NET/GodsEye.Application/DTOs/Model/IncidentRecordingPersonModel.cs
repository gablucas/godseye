namespace GodsEye.Application.DTOs.Model
{
    public class IncidentRecordingPersonModel
    {
        public int PersonId { get; set; }
        public DateTime SeenAt { get; set; }
        public double VideoOffsetSeconds { get; set; }
    }
}
