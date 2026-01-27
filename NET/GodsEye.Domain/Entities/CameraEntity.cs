using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Domain.Entities
{
    public class CameraEntity : BaseEntity
    {
        public string Name { get; set; }
        public string? Connection { get; set; }
        public int? SectorId { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<int> Features { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
