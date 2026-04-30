using GodsEye.API.Enums;
using GodsEye.API.Interfaces;
using GodsEye.Shared;
using System.Text.Json.Serialization;

namespace GodsEye.API.DTO
{
    
    public class PersonCache : IGodsEyeCache
    {
        public int Id { get; set; }
        public float[] Embedding { get; set; }
        public int? LastCameraId { get; set; }
        public int? AccessLevelId { get; set; }
        public DateTime? LastSeen { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public readonly object SyncRoot = new();
    }

    public class CameraCache : IGodsEyeCache
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public List<FeatureCache> Features { get; set; }
    }

    public class FeatureCache : IJSonTypeList
    {
        public int Id { get; set; }
    }

    public class AccessLevelCache : IGodsEyeCache
    {
        public int Id { get; set; }
        public List<AccessLevelSectorRuleCache> Sectors { get; set; }

    }

    public class AccessLevelSectorRuleCache : IJSonTypeList
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }
    
}
