namespace GodsEye.Application.DTOs.Model
{
    public class CameraModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }
    }
}
