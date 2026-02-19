using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraConfigIncidentRecording
{
    public class GetCameraConfigIncidentRecordingHandler : IRequestHandler<GetCameraConfigIncidentRecordingRequest, ApiResponse<CameraConfigIncidentRecordingModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetCameraConfigIncidentRecordingHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CameraConfigIncidentRecordingModel>> Handle(GetCameraConfigIncidentRecordingRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CONFIG_INCIDENT_RECORDING_GET_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = request.CameraId
            };

            var result = await _context.QuerySingleSqlAsync<CameraConfigIncidentRecordingModel>(sql, parameters, cancellationToken);

            return ApiResponse<CameraConfigIncidentRecordingModel>.Ok(result);
        }
    }
}
