namespace GodsEye.Shared.Response.Camera
{
    public class CameraByFeatureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }
    }
}
