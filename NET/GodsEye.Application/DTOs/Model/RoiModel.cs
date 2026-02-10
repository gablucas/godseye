namespace GodsEye.Application.DTOs.Model
{
    public class RoiModel
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public List<PointModel> Points { get; set; } = new();
    }

    public class PointModel
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
