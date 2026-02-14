using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Application.DTOs.Model
{
    public class CameraModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public bool? Status { get; set;  }

        public List<FeatureDTO> Features { get; set; } = new();
    }

    public class FeatureDTO
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
    }
}
