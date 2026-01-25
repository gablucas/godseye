using AutoMapper;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Commands.CreateDwellTimeMonitoring
{
    public class CreateDwellTimeMonitoringMapper : Profile
    {
        public CreateDwellTimeMonitoringMapper()
        {
            CreateMap<CreateDwellTimeMonitoringRequest, DwellTimeMonitoringEntity>();
        }
    }
}
