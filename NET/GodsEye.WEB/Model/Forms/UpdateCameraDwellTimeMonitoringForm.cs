namespace GodsEye.WEB.Model.Forms
{
    public class UpdateCameraDwellTimeMonitoringForm
    {
        public int CameraId { get; set; }
        public int MaxDwellTime { get; set; }
        public int RecognitionGracePeriod { get; set; }
    }
}
