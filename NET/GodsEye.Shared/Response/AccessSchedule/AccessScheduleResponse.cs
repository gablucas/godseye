using GodsEye.Shared.Enums;

namespace GodsEye.Shared.Response.AccessSchedule
{
    public class AccessScheduleResponse : IJSonTypeList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<AccessScheduleRuleDTO> Rules { get; set; } = new();
    }

    public class AccessScheduleRuleDTO : IJSonTypeList
    {
        public int Id { get; set; }
        public WeekDayEnum WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
