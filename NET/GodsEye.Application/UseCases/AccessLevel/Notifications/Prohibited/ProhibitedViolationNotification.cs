using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Notifications.Prohibited
{
    public sealed record ProhibitedViolationNotification(int personId, int sectorId, DateTime identifiedAt) : INotification;
}
