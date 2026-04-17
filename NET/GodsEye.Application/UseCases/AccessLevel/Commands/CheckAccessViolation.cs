using GodsEye.Application.Interfaces;
using GodsEye.Application.UseCases.AccessLevel.Notifications.Prohibited;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Commands
{
    public sealed record CheckAccessViolationRequest(int cameraId, int personId, DateTime identifiedAt) : IRequest<Unit>;

    public class CheckAccessViolationHandler : IRequestHandler<CheckAccessViolationRequest, Unit>
    {
        private readonly IGodsEyeState _godEyeState;
        private readonly IMediator _mediator;

        public CheckAccessViolationHandler(IGodsEyeState godEyeState, IMediator mediator)
        {
            _godEyeState = godEyeState;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CheckAccessViolationRequest request, CancellationToken cancellationToken)
        {
            var person = _godEyeState.GetPersonById(request.personId);
            if (person is null) return Unit.Value;

            var camera = _godEyeState.GetCameraById(request.cameraId);
            if (camera is null) return Unit.Value;

            if (person.AccessLevelId is null) return Unit.Value;

            var accessLevel = _godEyeState.GetAccessLevelById(person.AccessLevelId.Value);
            if (accessLevel is null) return Unit.Value;

            var sectorRule = accessLevel.Sectors?.FirstOrDefault(x => x.Id == camera.SectorId);
            if (sectorRule is null) return Unit.Value;


            switch (sectorRule.RuleType)
            {
                case AccessLevelSectorRuleEnum.PROHIBITED:
                    await _mediator.Publish(new ProhibitedViolationNotification(person.Id, sectorRule.Id, request.identifiedAt), cancellationToken);
                    break;

                case AccessLevelSectorRuleEnum.BLACKLIST:
                    break;

            }

            return Unit.Value;
        }
    }
}
