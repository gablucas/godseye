using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.CreateNotificationGroup
{
    public class NotificationGroupMapper : Profile
    {
        public NotificationGroupMapper() 
        {
            CreateMap<CreateNotificationGroupRequest, NotificationGroupEntity>();
        }
    }
}
