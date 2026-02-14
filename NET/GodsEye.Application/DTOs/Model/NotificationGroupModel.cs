using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class NotificationGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? EmailsJson { get; set; }

        public List<EmailDTO> Emails { get; set; } = new();

    }
}
