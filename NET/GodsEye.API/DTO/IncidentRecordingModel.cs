using GodsEye.API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.API.DTO
{
    public class IncidentRecordingModel
    {
        public int Id { get; set; }
        public string? Sector { get; set; }
        public DateTime IncidentTime { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
        public IncidentStatusEnum Status { get; set; }
        public string? FileName { get; set; }

        public List<IncidentRecordingPerson> Persons { get; set; } = new();
    }

    public class IncidentRecordingPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public DateTime SeenAt { get; set; }
        public double VideoOffsetSeconds { get; set; }
    }
}
