namespace GodsEye.Shared.Response.NotificationGroups
{
    public class NotificationGroupsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? EmailsJson { get; set; }

        public List<EmailDTO> Emails { get; set; } = new();
    }

    public class EmailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
