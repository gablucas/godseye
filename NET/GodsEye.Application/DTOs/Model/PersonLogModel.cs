using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Application.DTOs.Model
{
    public class PersonLogModel
    {
        public string Sector { get; set; }
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }
    }
}
