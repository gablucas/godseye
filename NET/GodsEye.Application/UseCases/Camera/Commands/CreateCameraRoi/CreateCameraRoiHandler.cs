using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi
{
    public class CreateCameraRoiHandler : IRequestHandler<CreateCameraRoiRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateCameraRoiHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(CreateCameraRoiRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_ROI_CREATE(@P_CAMERA_ID, @P_ROI_TYPE, @P_COORDINATES_JSON)";

            var parameters = new Dictionary<string, object?> 
            {
                ["@P_CAMERA_ID"] = request.CameraId,
                ["@P_ROI_TYPE"] = request.RoiType,
                ["@P_COORDINATES_JSON"] = JsonSerializer.Serialize(request.Coordinates),
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(query, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao cadastra o roi da câmera");
        }
    }
}
