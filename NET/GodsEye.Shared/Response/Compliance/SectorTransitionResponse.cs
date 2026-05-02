namespace GodsEye.Shared.Response.Compliance
{
    public class SectorTransitionResponse
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public List<SectorTransitionRuleResponse> Rules { get; set; }
    }

    public class SectorTransitionRuleResponse : IJSonTypeList
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
    }
}
