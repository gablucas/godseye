using GodsEye.Domain.Enums;

namespace GodsEye.Application.DTOs.Model
{
    public class AccessLevelModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public List<SectorAccessLevelDTO> Sectors { get; set; } = new();
        public AccessLevelScheduleDTO SectorSchedule { get; set; }
        public List<RoutineAccessLevelDTO> Routines { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SectorAccessLevelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }

    public class AccessLevelScheduleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AccessLevelScheduleRuleDTO> Rules { get; set; } = new();
    }

    public class AccessLevelScheduleRuleDTO
    {
        public WeekDayEnum WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
    
    public class RoutineAccessLevelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
