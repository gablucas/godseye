using GodsEye.Domain.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class RoutineForm
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public RoutineRuleTypeEnum RuleType { get; set; } = RoutineRuleTypeEnum.SectorTransition;

        public List<RoutineRuleSectorTransition> Rules { get; set; } = new();

        public bool HasSector(int? sectorId, RoutineRuleSectorTransition ignore = null)
        {
            return Rules
                .Where(x => x != ignore)
                .Any(x => x.SectorId == sectorId);
        }

        public void AddNewRule()
        {
            var orderIndex = Rules.Count() + 1;

            Rules.Add(new RoutineRuleSectorTransition() { OrderIndex = orderIndex });
        }
    }

    public class RoutineRuleSectorTransition
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int? SectorId { get; set; }
    }
}
