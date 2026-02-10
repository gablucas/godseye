namespace GodsEye.WEB.Model.Forms
{
    public class UpdateCameraRecognitionForm
    {
        public int CameraId { get; set; }
        public Rect FaceDimension { get; set; } = new();
        public Rect CameraDimension { get; set; } = new();
    }

    public class Rect
    {
        // Usado para o Retângulo (Face) e como Bounding Box do Polígono
        public float Width { get; set; }
        public float Height { get; set; }

        public List<Point> Points { get; set; } = new();
    }

    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
