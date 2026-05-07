using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Shared.Response.Sector
{
    public class SectorResponse : IBaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int IsActive { get; set; }

        [NotMapped]
        public List<SectorResponse> Children { get; set; } = new();
    }
}
