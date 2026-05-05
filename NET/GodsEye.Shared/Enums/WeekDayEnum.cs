using System.ComponentModel.DataAnnotations;

namespace GodsEye.Shared.Enums
{
    public enum WeekDayEnum
    {
        [Display(Name = "Segunda-feira")]
        Monday = 0,

        [Display(Name = "Terça-feira")]
        Tuesday = 1,

        [Display(Name = "Quarta-feira")]
        Wednesday = 2,

        [Display(Name = "Quinta-feira")]
        Thursday = 3,

        [Display(Name = "Sexta-feira")]
        Friday = 4,

        [Display(Name = "Sábado")]
        Saturday = 5,

        [Display(Name = "Domingo")]
        Sunday = 6
    }
}
