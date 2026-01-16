using AutoMapper;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCamera
{
    public class CreateCameraMapper : Profile
    {
        public CreateCameraMapper()
        {
            CreateMap<CreateCameraRequest, CameraEntity>();
        }
    }
}
