using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraById
{
    public class GetCameraByIdHandler : IRequestHandler<GetCameraByIdRequest, ApiResponse<CameraModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetCameraByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CameraModel>> Handle(GetCameraByIdRequest request, CancellationToken cancellationToken)
        {
            const string sql = "CALL SP_CAMERA_GET_BY_ID(@P_CAMERA_ID)";

            var parameters = new 
            { 
                P_CAMERA_ID = request.cameraId 
            };

            var camera = await _context.QuerySingleSqlAsync<CameraModel>(sql, parameters, cancellationToken);

            if (camera == null)
            {
                return ApiResponse<CameraModel>.Fail(404, "Câmera não encontrada.");
            }

            return ApiResponse<CameraModel>.Ok(camera);
        }
    }
}
