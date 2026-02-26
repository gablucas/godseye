using GodsEye.Domain.Enums;

namespace GodsEye.Application.DTOs.Model
{
    public class AccessScheduleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<AccessScheduleRuleModel> Rules { get; set; } = new();
    }

    public class AccessScheduleRuleModel
    {
        public int Id { get; set; }
        public WeekDayEnum WeekDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
