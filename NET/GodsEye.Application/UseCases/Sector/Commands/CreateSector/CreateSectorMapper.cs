using AutoMapper;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.Sector.Commands.CreateSector
{
    public class CreateSectorMapper : Profile
    {
        public CreateSectorMapper()
        {
            CreateMap<CreateSectorRequest, SectorEntity>();
        }
    }
}
