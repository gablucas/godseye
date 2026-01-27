namespace GodsEye.WEB.Model.Forms
{
    public class UpdateCameraRecognitionForm
    {
        public int CameraId { get; set; }
        public Rect FaceDimension { get; set; } = new();
        public Rect CameraDimension { get; set; } = new();
    }

    public class Rect()
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
