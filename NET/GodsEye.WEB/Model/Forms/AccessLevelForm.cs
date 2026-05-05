using GodsEye.Shared.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class AccessLevelForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SectorAccessLevelForm> Sectors { get; set; } = new();
        public int? AccessScheduleId { get; set; }
    }
    public class SectorAccessLevelForm
    {
        public int SectorId { get; set; }
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }
}
