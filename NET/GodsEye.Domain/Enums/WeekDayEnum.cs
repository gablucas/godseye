using System.ComponentModel;

namespace GodsEye.Domain.Enums
{
    public enum WeekDayEnum
    {
        [Description("Domingo")]
        Sunday = 0,

        [Description("Segunda-feira")]
        Monday = 1,

        [Description("Terça-feira")]
        Tuesday = 2,

        [Description("Quarta-feira")]
        Wednesday = 3,

        [Description("Quinta-feira")]
        Thursday = 4,

        [Description("Sexta-feira")]
        Friday = 5,

        [Description("Sábado")]
        Saturday = 6
    }
}
