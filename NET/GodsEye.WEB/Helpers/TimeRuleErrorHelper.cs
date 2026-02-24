using GodsEye.Domain.Enums;

namespace GodsEye.WEB.Helpers
{
    public class TimeRuleErrorHelper
    {
        public TimeRuleErrorHelper(int index, WeekDayEnum day, bool isStartTimeWrong, bool isEndTimeWrong, string? message)
        {
            Index = index;
            Day = day;
            IsStartTimeWrong = isStartTimeWrong;
            IsEndTimeWrong = isEndTimeWrong;
            Message = message;
        }

        public int Index { get; set; }
        public WeekDayEnum Day { get; set; }
        public bool IsStartTimeWrong { get; set; }
        public bool IsEndTimeWrong { get; set; }
        public string? Message { get; set; }
    }
}
