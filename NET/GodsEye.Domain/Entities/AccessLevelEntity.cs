namespace GodsEye.Domain.Entities
{
    public class AccessLevelEntity : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int AccessScheduleId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<AccessLevelSectorEntity> Sectors { get; set; } = new();

    }
}
