
namespace GodsEye.Domain.Entities
{
    public class NotificationGroupEntity : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<string> Emails { get; set;}
    }
}
