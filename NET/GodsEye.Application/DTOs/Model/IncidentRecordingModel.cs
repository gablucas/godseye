using GodsEye.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
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
        public string? PersonsJSON { get; set; }

        public List<IncidentRecordingPersonDTO> Persons
        {
            get => string.IsNullOrWhiteSpace(PersonsJSON)
                ? new List<IncidentRecordingPersonDTO>()
                : JsonSerializer.Deserialize<List<IncidentRecordingPersonDTO>>(PersonsJSON);
        }
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
