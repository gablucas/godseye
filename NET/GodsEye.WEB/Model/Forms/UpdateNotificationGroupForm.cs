

using GodsEye.Shared.Response;

namespace GodsEye.WEB.Model.Forms
{
    public class UpdateNotificationGroupForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LookupResponse> Emails { get; set; } = new List<LookupResponse>();
        public List<string> NewEmails { get; set; } = new();
        public List<int> RemoveEmails { get; set; } = new();
    }
}
