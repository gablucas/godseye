using GodsEye.Shared.Enums;
using GodsEye.Shared.Interfaces;

namespace GodsEye.Shared.Response.AccessLevel
{
    public class AccessLevelResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public List<SectorAccessLevelDTO> Sectors { get; set; } = new();
        public AccessLevelScheduleDTO SectorSchedule { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SectorAccessLevelDTO : IJSonTypeList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }

    public class AccessLevelScheduleDTO : IJsonType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AccessLevelScheduleRuleDTO> Rules { get; set; } = new();
    }

    public class AccessLevelScheduleRuleDTO : IJSonTypeList, IJsonType
    {
        public WeekDayEnum WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
