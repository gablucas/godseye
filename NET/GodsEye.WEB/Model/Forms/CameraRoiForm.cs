using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class CameraRoiForm
    {
        public int Id { get; set; } = 0;
        public RoiTypeEnum RoiType { get; set; }
        public RoiModel Coordinates { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
