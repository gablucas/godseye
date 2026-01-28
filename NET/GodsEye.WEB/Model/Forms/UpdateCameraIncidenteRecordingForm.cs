namespace GodsEye.WEB.Model.Forms
{
    public class UpdateCameraIncidenteRecordingForm
    {
        public int CameraId { get; set; }
        public string MacAddress { get; set; }
        public List<string> Emails { get; set; } = new();
    }
}
