using GodsEye.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Shared.Response.Camera
{
    public class CameraResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public bool? Status { get; set; }

        public List<FeatureResponse> Features { get; set; } = new();
    }

    public class FeatureResponse : IJSonTypeList
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
    }

}
