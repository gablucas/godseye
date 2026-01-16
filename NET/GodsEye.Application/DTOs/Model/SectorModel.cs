using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class SectorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
        public string? CamerasJson { get; set; }

        public List<CameraDTO> Cameras
        {
            get => string.IsNullOrWhiteSpace(CamerasJson)
                ? new List<CameraDTO>()
                : JsonSerializer.Deserialize<List<CameraDTO>>(CamerasJson);
        }
    }

    public class CameraDTO
    {
        public int CameraId { get; set; }
        public string CameraName { get; set; }
    }
}
