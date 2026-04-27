using GodsEye.Shared.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class CameraRoiForm
    {
        public int Id { get; set; } = 0;
        public RoiTypeEnum RoiType { get; set; }
        public RoiForm Coordinates { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class RoiForm
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public List<PointForm> Points { get; set; } = new();
    }

    public class PointForm
    {
        public float X { get; set; }
        public float Y { get; set; }

    }
}
