namespace GodsEye.Application.DTOs.Model
{
    public class DwellTimeMonitoringDetailsModel
    {
        public string PersonName { get; set; }
        public string ImagePath { get; set; }
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
    }
}
