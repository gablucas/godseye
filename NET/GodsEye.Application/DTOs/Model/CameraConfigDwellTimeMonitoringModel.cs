namespace GodsEye.Application.DTOs.Model
{
    public class CameraConfigDwellTimeMonitoringModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public int MaxDwellTimeMinutes { get; set; }
        public int MaxNonIdentificationTimeMinutes { get; set; }
    }
}
