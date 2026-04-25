using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.Camera
{
    public class CameraRoiResponse
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public RoiTypeEnum RoiType { get; set; }
        public RoiDTO Coordinates { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoiDTO
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public List<PointDTO> Points { get; set; } = new();
    }

    public class PointDTO
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
