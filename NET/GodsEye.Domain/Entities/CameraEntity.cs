using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Domain.Entities
{
    public class CameraEntity : BaseEntity
    {
        public string Name { get; set; }
        public string? Connection { get; set; }
        [Column("SECTOR_ID")]
        public int? SectorId { get; set; }
    }
}
