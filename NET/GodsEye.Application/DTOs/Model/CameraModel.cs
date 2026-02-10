using System.ComponentModel.DataAnnotations.Schema;
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
        public bool IsActive { get; set; }

        [NotMapped]
        public bool? Status { get; set;  }

        public string? FeaturesJson { get; set; }

        public List<FeatureDTO> Features
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FeaturesJson))
                    return new List<FeatureDTO>();

                try
                {
                    return JsonSerializer.Deserialize<List<FeatureDTO>>(FeaturesJson)
                           ?? new List<FeatureDTO>();
                }
                catch
                {
                    // opcional: logar o erro
                    return new List<FeatureDTO>();
                }
            }
        }
    }

    public class FeatureDTO
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
    }
}
