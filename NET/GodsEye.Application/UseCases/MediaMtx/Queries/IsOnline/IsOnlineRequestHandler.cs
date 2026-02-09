using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.MediaMtx.Queries.IsOnline
{
    public class IsOnlineRequestHandler : IRequestHandler<IsOnlineRequest, ApiResponse<bool>>
    {
        private readonly IMediaMtxService _mediaMtxService;

        public IsOnlineRequestHandler(IMediaMtxService mediaMtxService)
        {
            _mediaMtxService = mediaMtxService;
        }

        public async Task<ApiResponse<bool>> Handle(IsOnlineRequest request, CancellationToken cancellationToken)
        {
            var isOnline = await _mediaMtxService.IsOnlineAsync();
            return ApiResponse<bool>.Ok(isOnline);
        }
    }
}
