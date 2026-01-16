using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Application.DTOs.Model
{
    public class EnvironmentMonitoringModel
    {
        public string Person { get; set; }
        public string PersonPhoto { get; set; }
        public string Sector { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
    }
}
