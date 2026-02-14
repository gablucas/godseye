using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCameras
{
    public class GetAllCamerasHandler : IRequestHandler<GetAllCamerasRequest, ApiResponse<IEnumerable<CameraModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCamerasHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CameraModel>>> Handle(GetAllCamerasRequest request, CancellationToken cancellationToken)
        {
            const string sql = "CALL SP_CAMERA_GET_ALL()";

            var cameras = await _context.QuerySqlAsync<CameraModel>(sql, cancellationToken);

            return ApiResponse<IEnumerable<CameraModel>>.Ok(cameras);
        }
    }
}
