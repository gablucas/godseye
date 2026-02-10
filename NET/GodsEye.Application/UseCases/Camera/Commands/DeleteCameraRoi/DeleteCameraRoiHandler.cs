using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.DeleteCameraRoi
{
    public class DeleteCameraRoiHandler : IRequestHandler<DeleteCameraRoiRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCameraRoiHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(DeleteCameraRoiRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_ROI_DELETE(@P_CAMERA_ROI_ID)";

            var parameters = new Dictionary<string, object?>
            {
                ["@P_CAMERA_ROI_ID"] = request.cameraRoiId
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(query, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao deletar o roi da câmera");
        }
    }
}
