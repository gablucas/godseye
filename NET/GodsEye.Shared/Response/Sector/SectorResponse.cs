namespace GodsEye.Shared.Response.Sector
{
    public class SectorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }

        public List<CameraSectorResponse> Cameras { get; set; } = new();

        public List<NotificationGroupResponse> NotificationGroups { get; set; } = new();
    }

    public class CameraSectorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NotificationGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
