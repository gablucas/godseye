namespace GodsEye.Application.DTOs.Model
{
    public class SectorTransitionRuleDTO
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int? SectorId { get; set; }
    }
}
