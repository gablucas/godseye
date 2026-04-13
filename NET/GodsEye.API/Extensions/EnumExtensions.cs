using System.ComponentModel.DataAnnotations;

namespace GodsEye.API.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                             .Cast<DisplayAttribute>()
                             .FirstOrDefault();
            return attr?.Name ?? value.ToString();
        }
    }
}
