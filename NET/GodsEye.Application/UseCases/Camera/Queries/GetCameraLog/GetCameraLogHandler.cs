using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetCameraLog
{
    public class GetCameraLogHandler : IRequestHandler<GetCameraLogRequest, ApiResponse<IEnumerable<CameraLogModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCameraLogHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CameraLogModel>>> Handle(GetCameraLogRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_ENVIRONMENT_GET_MONITORING_LOG(@P_CAMERA_ID)";

            var parameters = new 
            { 
                P_CAMERA_ID = request.cameraId 
            };

            var camera = await _context.QuerySqlAsync<CameraLogModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<CameraLogModel>>.Ok(camera);
        }
    }
}
