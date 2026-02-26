using GodsEye.Domain.Enums;

namespace GodsEye.Domain.Entities
{
    public class AccessLevelSectorEntity
    {
        public int AccessLevelId { get; set; }
        public int SectorId { get; set; }
        public AccessLevelSectorRuleEnum RuleType { get; set; }

    }
}
