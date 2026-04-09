namespace GodsEye.Application.DTOs.Model
{
    public class GodsEyeCache
    {
        public List<PersonCache> Persons { get; set; } = new();
        public List<CameraCache> Cameras { get; set; } = new();
    }

    public class PersonCache
    {
        public int Id { get; set; }
        public float[] Embedding { get; set; }
        public int? LastCameraId { get; set; }
        public DateTime? LastSeen { get; set; }
    }

    public class CameraCache
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public List<FeatureCache> Features { get; set; }
    }

    public class FeatureCache
    {
        public int Id { get; set; }
    }
}
