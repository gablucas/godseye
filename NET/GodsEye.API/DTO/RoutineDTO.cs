using GodsEye.Domain.Enums;

namespace GodsEye.API.DTO
{
    public class RoutineDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComplianceRuleEnum Type { get; set; }
        public List<RoutineRuleSectorTransitionDTO> Rules { get; set; }
    }

    public class RoutineRuleSectorTransitionDTO
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int SectorId { get; set; }
    }
}
