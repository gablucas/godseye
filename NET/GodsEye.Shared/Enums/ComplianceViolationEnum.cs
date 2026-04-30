using System.ComponentModel.DataAnnotations;

namespace GodsEye.Shared.Enums
{
    public enum ComplianceViolationEnum
    {
        [Display(Name = "Ultrapassou o tempo permitido")]
        EXCEEDED_ALLOWED_TIME,
    }
}
