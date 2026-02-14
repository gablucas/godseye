using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCamera
{
    public class UpdateCameraHandler : IRequestHandler<UpdateCameraRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCameraHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateCameraRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_UPDATE(@P_CAMERA_ID, @P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)";

            var parameters = new
            {
                P_CAMERA_ID = request.id,
                P_NAME = request.Name,
                P_CONNECTION = request.Connection,
                P_SECTOR_ID = request.SectorId,
                P_FEATURES_JSON = request.Features

            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
