using GodsEye.Shared.Interfaces;

namespace GodsEye.Shared.Response.EnvironmentMonitoring
{
    public class GetEnviromentMonitoringPerSectorResponse : IJSonTypeList
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int TotalPerson { get; set; }
        public List<EnvironmentMonitoringResponse>? EnvironmentMonitoringLog { get; set; } = new();
    };
}
