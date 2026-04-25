using MediatR;

namespace GodsEye.Shared
{
    public sealed record EnvironmentMonitoringNotification(int CameraId, int PersonId, float Score, DateTime IdentifiedAt) : INotification;
}
