using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi
{
    public class UpdateCameraRoiHandler : IRequestHandler<UpdateCameraRoiRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCameraRoiHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(UpdateCameraRoiRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_ROI_UPDATE(@P_CAMERA_ROI_ID, @P_COORDINATES_JSON, @P_IS_ACTIVE)";

            var parameters = new 
            {
                P_CAMERA_ROI_ID = request.CameraRoiId,
                P_COORDINATES_JSON = JsonSerializer.Serialize(request.Coordinates),
                P_IS_ACTIVE = request.IsActive
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(query, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao atualizar o roi da câmera");
        }
    }
}
