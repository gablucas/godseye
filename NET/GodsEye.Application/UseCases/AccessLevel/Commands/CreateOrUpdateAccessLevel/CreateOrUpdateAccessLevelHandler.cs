using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel
{
    public class CreateOrUpdateAccessLevelHandler : IRequestHandler<CreateOrUpdateAccessLevelRequest, ApiResponse<int>>
    {
        private readonly IDapperContext _dapperContext;

        public CreateOrUpdateAccessLevelHandler(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<ApiResponse<int>> Handle(CreateOrUpdateAccessLevelRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_SECTORS_JSON, @P_ACCESS_SCHEDULE_ID)";

            var parameters = new
            {
                P_ID = request.Id,
                P_NAME = request.Name,
                P_SECTORS_JSON = JsonSerializer.Serialize(request.Sectors),
                P_ACCESS_SCHEDULE_ID = request.AccessScheduleId
            };

            var result = await _dapperContext.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao cadastra o nível de acesso");
        }
    }
}
