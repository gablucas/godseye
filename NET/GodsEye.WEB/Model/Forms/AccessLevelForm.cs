namespace GodsEye.WEB.Model.Forms
{
    public class AccessLevelForm
    {
        public string Name { get; set; }
        public IEnumerable<int> Sectors { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<int> NotAllowedSectors { get; set; } = Enumerable.Empty<int>();
        public int? AccessLevel { get; set; }
    }
}
