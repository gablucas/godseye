namespace GodsEye.Application.DTOs.Model
{
    public class EnvironmentMonitoringPersonModel
    {
        public string PersonName { get; set; }
        public string PersonPhoto { get; set;  }

        public List<EnvironmentMonitoringPersonLog> Logs { get; set; } = new();
    }

    public class EnvironmentMonitoringPersonLog
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? TimeOnSector { get; set; }

    }
}
