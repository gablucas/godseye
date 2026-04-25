using GodsEye.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.API.DTO
{
    public class IncidentRecordingDTO
    {
        public int Id { get; set; }
        public string? Sector { get; set; }
        public DateTime IncidentTime { get; set; }
        [Column("CREATED_AT")]
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
