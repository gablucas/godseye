using GodsEye.Shared.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class AccessScheduleForm
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public bool IsActive { get; set; }
        public List<ScheduleRuleDTO> Rules { get; set; } 
    }

    public class ScheduleRuleDTO
    {
        public int Id { get; set; }

        public WeekDayEnum WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
