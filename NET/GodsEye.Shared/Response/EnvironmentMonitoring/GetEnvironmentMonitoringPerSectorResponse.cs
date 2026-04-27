namespace GodsEye.Shared.Response.EnvironmentMonitoring
{
    public class GetEnviromentMonitoringPerSectorResponse 
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int TotalPerson { get; set; }
        public List<EnvironmentMonitoringLogResponse>? EnvironmentMonitoringLog { get; set; } = new();
    };
}
