using GodsEye.Domain.Enums;

namespace GodsEye.Application.DTOs.Model
{
    public class CameraRoiModel
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public RoiTypeEnum RoiType { get; set; }
        public RoiModel Coordinates { get; set; }
    }
}
