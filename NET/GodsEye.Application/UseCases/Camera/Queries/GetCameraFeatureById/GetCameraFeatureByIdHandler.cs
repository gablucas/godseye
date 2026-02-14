using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraFeatureById
{
    public class GetCameraFeatureByIdHandler : IRequestHandler<GetCameraFeatureByIdRequest, ApiResponse<IEnumerable<CameraFeatureModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCameraFeatureByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CameraFeatureModel>>> Handle(GetCameraFeatureByIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_GET_FEATURES_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new 
            { 
                P_CAMERA_ID = request.cameraId 
            };

            var camera = await _context.QuerySqlAsync<CameraFeatureModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<CameraFeatureModel>>.Ok(camera);
        }
    }
}
