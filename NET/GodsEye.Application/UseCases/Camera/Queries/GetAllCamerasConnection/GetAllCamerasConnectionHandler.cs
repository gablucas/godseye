using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCamerasConnection
{
    public class GetAllCamerasConnectionHandler : IRequestHandler<GetAllCamerasConnectionRequest, ApiResponse<IEnumerable<CameraConnectionModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCamerasConnectionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CameraConnectionModel>>> Handle(GetAllCamerasConnectionRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_GET_ALL_CAMERA_CONNECTION()";

            var cameras = await _context.QuerySqlAsync<CameraConnectionModel>(sql, cancellationToken);

            return ApiResponse<IEnumerable<CameraConnectionModel>>.Ok(cameras);
        }
    }
}
