namespace GodsEye.WEB.Model.Forms
{
    public class CameraDwellTimeMonitoringForm
    {
        public int Id { get; set; } = 0;
        public int CameraId { get; set; }
        public int MaxDwellTimeMinutes { get; set; }
        public int MaxNonIdentificationTimeMinutes { get; set; }
    }
}
