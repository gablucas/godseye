namespace GodsEye.Shared.Response.EnvironmentMonitoring
{
    public class EnvironmentMonitoringPersonResponse
    {
        public string PersonName { get; set; }
        public string PersonPhoto { get; set; }
        public List<EnvironmentMonitoringPersonLogResponse> Logs { get; set; } = new();
    }

    public class EnvironmentMonitoringPersonLogResponse
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? TimeOnSector { get; set; }

    }
}
