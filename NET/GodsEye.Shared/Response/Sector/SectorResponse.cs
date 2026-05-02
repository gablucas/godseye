namespace GodsEye.Shared.Response.Sector
{
    public class SectorResponse : IBaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }

        public List<LookupResponse> Cameras { get; set; } = new();

        public List<LookupResponse> NotificationGroups { get; set; } = new();
    }
}
