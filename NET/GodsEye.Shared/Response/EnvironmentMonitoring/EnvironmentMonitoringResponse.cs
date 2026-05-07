using GodsEye.Shared.Interfaces;

namespace GodsEye.Shared.Response.EnvironmentMonitoring
{
    public sealed record EnvironmentMonitoringResponse : IJSonTypeList
    {
        public int PersonId { get; set; }
        public string Person { get; set; }
        public string PersonPhoto { get; set; }
        public int SectorId { get; set; }
        public string Sector { get; set; }
        public DateTime? IdentifiedAt { get; set; }
    }
}
