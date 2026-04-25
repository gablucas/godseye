using GodsEye.Domain.Enums;

namespace GodsEye.Application.DTOs.Model
{
    public class RoutineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComplianceRuleEnum Type { get; set; }
        public List<RoutineRuleSectorTransitionModel> Rules { get; set; }
    }

    public class RoutineRuleSectorTransitionModel
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int SectorId { get; set; }
    }
}
