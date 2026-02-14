using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class SectorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }

        public List<CameraDTO> Cameras { get; set; } = new();

        public List<NotificationGroupDTO> NotificationGroups { get; set; } = new ();
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
