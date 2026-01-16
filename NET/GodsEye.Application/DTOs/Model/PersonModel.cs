using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Active { get; set; }

        public string? SectorsJson { get; set; }

        public List<SectorDTO> Sectors
        {
            get => string.IsNullOrWhiteSpace(SectorsJson)
                ? new List<SectorDTO>()
                : JsonSerializer.Deserialize<List<SectorDTO>>(SectorsJson);
        }
    }

    public class SectorDTO
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; }
    }
}
