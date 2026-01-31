using GodsEye.Application.DTOs.Model;

namespace GodsEye.WEB.Model.Forms
{
    public class UpdateNotificationGroupForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EmailDTO> Emails { get; set; } = new List<EmailDTO>();
        public List<string> NewEmails { get; set; } = new();
        public List<int> RemoveEmails { get; set; } = new();
    }
}
