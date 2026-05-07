namespace GodsEye.WEB.Model.Forms
{
    public class CreateSectorForm
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<int> NotificationGroups { get; set; } = new List<int>();
    }
}
