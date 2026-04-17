using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Notifications.Prohibited
{
    public class SendEmailProhibitedHandler : INotificationHandler<ProhibitedViolationNotification>
    {
        private readonly IAccessViolationQuerie _accessViolationQuerie;
        private readonly IEmailService _emailService;

        public SendEmailProhibitedHandler(IAccessViolationQuerie accessViolationQuerie, IEmailService emailService)
        {
            _accessViolationQuerie = accessViolationQuerie;
            _emailService = emailService;
        }

        public async Task Handle(ProhibitedViolationNotification notification, CancellationToken cancellationToken)
        {
            var result = await _accessViolationQuerie.GetAccessViolationDetail(notification.personId, notification.sectorId, cancellationToken);

            if (result is null) return;

            var html = await _emailService.LoadTemplateAsync(
                "AccessViolationProhibited.html",
                new Dictionary<string, string>
                {
                    ["date"] = notification.identifiedAt.ToString(),
                    ["person"] = result.Person,
                    ["sector"] = result.Sector,
                }
            );

            //await _emailService.SendAsync(["gabriel.pegoretti96@gmail.com"], "Notificação Intellivision: Acesso a um setor proibido", html);


            //await _emailService.SendAsync("gabriel.pegoretti96@gmail.com");

        }
    }
}
