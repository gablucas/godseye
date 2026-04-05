using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Extensions;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Routine.Commands.CreateRoutine
{
    public class CreateRoutineHandler : IRequestHandler<CreateRoutineRequest, ApiResponse<ProcedureResult>>
    {
        public readonly IDapperContext _context;

        public CreateRoutineHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateRoutineRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_TYPE, @P_ROUTINE_RULES_JSON)";

            var parameters = new
            {
                P_ID = request.Id,
                P_NAME = request.Name,
                P_TYPE = request.RuleType.GetDescription(),
                P_ROUTINE_RULES_JSON = JsonSerializer.Serialize(request.Rules),
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
