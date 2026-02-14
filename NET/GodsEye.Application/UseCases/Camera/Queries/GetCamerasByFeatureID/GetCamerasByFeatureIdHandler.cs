using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCamerasByFeatureID
{
    public class GetCamerasByFeatureIdHandler : IRequestHandler<GetCamerasByFeatureIdRequest, ApiResponse<IEnumerable<CameraByFeatureModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCamerasByFeatureIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CameraByFeatureModel>>> Handle(GetCamerasByFeatureIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_GET_BY_FEATURE_ID(@P_FEATURE_ID)";

            var parameters = new 
            { 
                P_FEATURE_ID = request.featureId 
            };

            var camera = await _context.QuerySqlAsync<CameraByFeatureModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<CameraByFeatureModel>>.Ok(camera);
        }
    }
}
