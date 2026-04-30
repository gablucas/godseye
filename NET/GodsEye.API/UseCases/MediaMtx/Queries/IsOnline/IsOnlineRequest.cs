using MediatR;

namespace GodsEye.API.UseCases.MediaMtx.Queries.IsOnline
{
    public sealed record IsOnlineRequest : IRequest<bool>;
}
