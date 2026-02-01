namespace GodsEye.WEB.Model.Forms
{
    public class CreateSectorForm
    {
        public string Name { get; set; }
        public IEnumerable<string> NotificationGroups { get; set; } = Enumerable.Empty<string>();
    }
}
