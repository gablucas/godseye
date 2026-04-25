using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.IncidentRecording
{
    public class IncidentRecordingResponse
    {
        public int Id { get; set; }
        public string? Sector { get; set; }
        public DateTime IncidentTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public IncidentStatusEnum Status { get; set; }
        public string? FileName { get; set; }

        public List<IncidentRecordingPersonDTO> Persons { get; set; } = new();
    }

    public class IncidentRecordingPersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public DateTime SeenAt { get; set; }
        public double VideoOffsetSeconds { get; set; }
    }
}
