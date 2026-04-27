using MediatR;

namespace GodsEye.Application.UseCases.MediaMtx.Queries.IsOnline
{
    public sealed record IsOnlineRequest : IRequest<bool>;
}
