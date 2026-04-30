using GodsEye.Shared.Interfaces;

namespace GodsEye.Shared.Response.NotificationGroups
{
    public class NotificationGroupsResponse : IJSonTypeList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? EmailsJson { get; set; }

        public List<LookupResponse> Emails { get; set; } = new();
    }
}
