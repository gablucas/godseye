using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class EnvironmentMonitoringPersonModel
    {
        public string PersonName { get; set; }
        public string PersonPhoto { get; set;  }
        public string? LogsJSON { get; set; }

        public List<EnvironmentMonitoringPersonLog> Logs
        {
            get => string.IsNullOrWhiteSpace(LogsJSON)
                ? new List<EnvironmentMonitoringPersonLog>()
                : JsonSerializer.Deserialize<List<EnvironmentMonitoringPersonLog>>(LogsJSON);
        }
    }

    public class EnvironmentMonitoringPersonLog
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? TimeOnSector { get; set; }

    }
}
