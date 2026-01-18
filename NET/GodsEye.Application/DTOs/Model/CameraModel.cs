using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class CameraModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }

        public string? FeaturesJson { get; set; }

        public List<FeatureDTO> Features
        {
            get => string.IsNullOrWhiteSpace(FeaturesJson)
                ? new List<FeatureDTO>()
                : JsonSerializer.Deserialize<List<FeatureDTO>>(FeaturesJson);
        }
    }

    public class FeatureDTO
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
    }
}
