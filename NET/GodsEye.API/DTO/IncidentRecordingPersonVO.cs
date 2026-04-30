namespace GodsEye.API.ValueObjects
{
    public class IncidentRecordingPersonVO
    {
        public int PersonId { get; set; }
        public DateTime SeenAt { get; set; }
        public double VideoOffsetSeconds { get; set; }
    }
}

