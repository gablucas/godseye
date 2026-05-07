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
        public int? LastDeviceId { get; set; }
        public int? AccessLevelId { get; set; }
        public DateTime? LastSeen { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public readonly object SyncRoot = new();
    }

    public class DeviceCache : IGodsEyeCache
    {
        public int Id { get; set; }
        public int OriginSectorId { get; set; }
        public int DestinationSectorId { get; set; }
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
