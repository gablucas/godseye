using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Application.DTOs.Model
{
    public class IncidentRecordingModel
    {
        public string? Person { get; set; }
        public string? PersonPhoto { get; set; }
        public string? Sector { get; set; }
        public DateTime IncidentTime { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
    }
}
