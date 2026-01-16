using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
    }
}
