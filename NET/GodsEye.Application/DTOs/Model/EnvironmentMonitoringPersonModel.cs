using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class EnvironmentMonitoringPersonModel
    {
        public string PersonName { get; set; }
        public string PersonPhoto { get; set;  }
        public string? LogsJSON { get; set; }

        public List<EnvironmentMonitoringLogs> Logs
        {
            get => string.IsNullOrWhiteSpace(LogsJSON)
                ? new List<EnvironmentMonitoringLogs>()
                : JsonSerializer.Deserialize<List<EnvironmentMonitoringLogs>>(LogsJSON);
        }
    }

    public class EnvironmentMonitoringLogs
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? TimeOnSector { get; set; }

    }
}
