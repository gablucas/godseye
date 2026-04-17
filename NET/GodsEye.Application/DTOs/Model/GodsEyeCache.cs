using GodsEye.Application.Interfaces;
using GodsEye.Domain.Enums;
using System.Text.Json.Serialization;

namespace GodsEye.Application.DTOs.Model
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

    public class FeatureCache
    {
        public int Id { get; set; }
    }

    public class AccessLevelCache : IGodsEyeCache
    {
        public int Id { get; set; }
        public List<AccessLevelSectorRuleCache> Sectors { get; set; }
        public List<AccessLevelRoutinesCache>? Routines { get; set; }

    }

    public class AccessLevelSectorRuleCache
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }

    public class AccessLevelRoutinesCache
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoutineRuleTypeEnum RuleType { get; set; }
        public List<RoutineRuleSectorTransitionCache> Rules { get; set; }
    }

    public class RoutineRuleSectorTransitionCache
    {
        public int OrderIndex { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
    }
}
