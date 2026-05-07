using GodsEye.Shared;
using GodsEye.API.Enums;
using MediatR;
using GodsEye.API.Interfaces;

namespace GodsEye.API.Features.EnvironmentMonitoring
{
    internal sealed class ValidateEnvironmentMonitoringRules(IDapperContext context, IGodsEyeState godEyeState, IMediator mediator, ILogger<ValidateEnvironmentMonitoringRules> logger) : INotificationHandler<EnvironmentMonitoringNotification>
    {
        public async Task Handle(EnvironmentMonitoringNotification notification, CancellationToken cancellationToken)
        {
            var person = godEyeState.GetPersonById(notification.PersonId);
            if (person is null) return;

            var camera = godEyeState.GetCameraById(notification.DeviceId);
            if (camera is null) return;

            if (person.AccessLevelId is null) return;

            var accessLevel = godEyeState.GetAccessLevelById(person.AccessLevelId.Value);
            if (accessLevel is null) return;

            var sectorRule = accessLevel.Sectors?.FirstOrDefault(x => x.Id == camera.DestinationSectorId);
            if (sectorRule is null) return;


            switch (sectorRule.RuleType)
            {
                //case AccessLevelSectorRuleEnum.PROHIBITED:
                //    await mediator.Publish(new ProhibitedViolationNotification(person.Id, sectorRule.Id, notification.IdentifiedAt), cancellationToken);
                //    break;

                case AccessLevelSectorRuleEnum.BLACKLIST:
                    break;
            }
        }
    }
}
