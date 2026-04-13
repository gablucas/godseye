using GodsEye.Domain.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class RoutineForm
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public RoutineRuleTypeEnum RuleType { get; set; } = RoutineRuleTypeEnum.SectorTransition;

        public List<RoutineRuleSectorTransitionForm> Rules { get; set; } = new();

        public bool HasSector(int? sectorId, RoutineRuleSectorTransitionForm ignore = null)
        {
            return Rules
                .Where(x => x != ignore)
                .Any(x => x.SectorId == sectorId);
        }

        public void AddNewRule()
        {
            var orderIndex = Rules.Count() + 1;

            Rules.Add(new RoutineRuleSectorTransitionForm() { OrderIndex = orderIndex });
        }
    }

    public class RoutineRuleSectorTransitionForm
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int? SectorId { get; set; }
    }
}
