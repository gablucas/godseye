namespace GodsEye.WEB.Model.Forms
{
    public class AccessLevelForm
    {
        public string Name { get; set; }
        public IEnumerable<int> AllowedSectors { get; set; } = new List<int>();
        public IEnumerable<int> BlacklistSectors { get; set; } = new List<int>();
        public int? AccessLevel { get; set; }
    }
}
