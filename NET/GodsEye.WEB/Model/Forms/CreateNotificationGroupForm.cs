namespace GodsEye.WEB.Model.Forms
{
    public class CreateNotificationGroupForm
    {
        public string Name { get; set; }
        public List<string> Emails { get; set; } = new();
    }
}
