namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public class SectorTransitionDTO
    {
        public int SectorId { get; set; }
        public int OrderIndex { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
    }
}
