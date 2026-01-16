using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Domain.Entities
{
    public class  PersonEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Embedding { get; set; }
        [Column("IMAGE_PATH")]
        public string ImagePath { get; set; }
        public int Active { get; set; }

        public IEnumerable<string> Sectors { get; set; }
    }
}
