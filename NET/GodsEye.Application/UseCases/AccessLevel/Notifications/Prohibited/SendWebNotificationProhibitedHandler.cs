using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Notifications.Prohibited
{
    public class SendWebNotificationProhibitedHandler : INotificationHandler<ProhibitedViolationNotification>
    {
        private readonly INotificationSignalR _notification;

        public SendWebNotificationProhibitedHandler(INotificationSignalR notification)
        {
            _notification = notification;
        }

        public async Task Handle(ProhibitedViolationNotification notification, CancellationToken cancellationToken)
        {
            await _notification.SendAlertNotification(1);
        }
    }
}
