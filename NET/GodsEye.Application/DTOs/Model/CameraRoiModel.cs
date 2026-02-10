using GodsEye.Domain.Enums;
using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class CameraRoiModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public RoiTypeEnum RoiType { get; set; }
        public RoiModel Coordinates
        {
            get => string.IsNullOrWhiteSpace(CoordinatesJSON)
                ? new RoiModel()
                : JsonSerializer.Deserialize<RoiModel>(CoordinatesJSON);
        }

        public string CoordinatesJSON { get; set; }
    }
}
