using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.CreateNotificationGroup
{
    public class CreateNotificationGroupHandler : IRequestHandler<CreateNotificationGroupRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly INotificationGroupRepository _notificationGroupRepository;

        public CreateNotificationGroupHandler(IMapper mapper, INotificationGroupRepository notificationGroupRepository)
        {
            _mapper = mapper;
            _notificationGroupRepository = notificationGroupRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateNotificationGroupRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<NotificationGroupEntity>(request);
            var result = await _notificationGroupRepository.Create(entity, cancellationToken);
            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
