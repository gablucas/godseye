using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraIncidentRecording
{
    public class UpdateCameraIncidentRecordingHandler : IRequestHandler<UpdateCameraIncidentRecordingRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCameraIncidentRecordingHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(UpdateCameraIncidentRecordingRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_INCIDENT_RECORDING_UPDATE(@P_CAMERA_ID, @P_MAC_ADDRESS)";

            var parameters = new
            {
                P_CAMERA_ID = request.CameraId,
                P_MAC_ADDRESS = request.MacAddress
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<int>.Ok(result.Id);
        }
    }
}
