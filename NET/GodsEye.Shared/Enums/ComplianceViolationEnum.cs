using System.ComponentModel.DataAnnotations;

namespace GodsEye.Shared.Enums
{
    public enum ComplianceViolationEnum
    {
        [Display(Name = "Ultrapassou o tempo permitido no setor")]
        EXCEEDED_ALLOWED_TIME,

        [Display(Name = "Não permaneceu o tempo mínimo definido no setor")]
        BELOW_MINIMUM_TIME,

        [Display(Name = "Não entrou no próximo setor no tempo definido")]
        DID_NOT_ENTER_NEXT_SECTOR_IN_TIME,
    }
}
