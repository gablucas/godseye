namespace GodsEye.Domain.Entities
{
    public class SectorEntity : BaseEntity
    {
        public string Name { get; set; }
        public IEnumerable<string> NotificationGroups { get; set; }
        public int Active { get; set; }
    }
}
