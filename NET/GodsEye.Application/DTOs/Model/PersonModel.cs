using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Active { get; set; }

        public List<SectorDTO> Sectors { get; set; } = new();
    }

    public class SectorDTO
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
    }
}
