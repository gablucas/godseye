namespace GodsEye.Application.DTOs.Model
{
    public class DwellTimeMonitoringModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
    }
}
