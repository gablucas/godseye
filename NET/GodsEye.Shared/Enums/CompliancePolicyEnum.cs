using System.ComponentModel.DataAnnotations;

namespace GodsEye.Shared.Enums
{
    public enum CompliancePolicyEnum
    {
        [Display(Name = "Transição de setores")]
        SECTOR_TRANSITION,

        [Display(Name = "EPI - Capacete")]
        EPI_HELMET,
    }
}
