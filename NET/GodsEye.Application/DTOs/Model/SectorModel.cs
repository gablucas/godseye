using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class SectorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
        public string? CamerasJson { get; set; }
        public string? NotificationGroupsJson { get; set; }

        public List<CameraDTO> Cameras
        {
            get => string.IsNullOrWhiteSpace(CamerasJson)
                ? new List<CameraDTO>()
                : JsonSerializer.Deserialize<List<CameraDTO>>(CamerasJson);
        }

        public List<NotificationGroupDTO> NotificationGroups
        {
            get => string.IsNullOrWhiteSpace(NotificationGroupsJson)
                ? new List<NotificationGroupDTO>()
                : JsonSerializer.Deserialize<List<NotificationGroupDTO>>(NotificationGroupsJson);
        }
    }

    public class CameraDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NotificationGroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
