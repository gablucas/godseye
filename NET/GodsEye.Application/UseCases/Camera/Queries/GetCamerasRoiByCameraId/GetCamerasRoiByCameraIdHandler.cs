using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCamerasRoiByCameraId
{
    public class GetCamerasRoiByCameraIdHandler : IRequestHandler<GetCamerasRoiByCameraIdRequest, ApiResponse<List<CameraRoiModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCamerasRoiByCameraIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CameraRoiModel>>> Handle(GetCamerasRoiByCameraIdRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_ROI_GET_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new Dictionary<string, object?>
            {
                ["@P_CAMERA_ID"] = request.cameraId
            };

            var result = await _context.QuerySqlAsync<CameraRoiModel>(query, parameters, cancellationToken);

            return ApiResponse<List<CameraRoiModel>>.Ok(result);
        }
    }
}
