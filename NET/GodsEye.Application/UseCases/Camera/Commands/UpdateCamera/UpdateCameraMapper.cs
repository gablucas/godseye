using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCamera
{
    public class UpdateCameraMapper : Profile
    {
        public UpdateCameraMapper()
        {
            CreateMap<UpdateCameraRequest, CameraEntity>();
        }
    }
}
