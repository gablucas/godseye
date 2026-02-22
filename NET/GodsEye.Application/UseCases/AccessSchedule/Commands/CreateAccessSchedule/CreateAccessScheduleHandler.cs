using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule
{
    public class CreateAccessScheduleHandler : IRequestHandler<CreateAccessScheduleRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateAccessScheduleHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(CreateAccessScheduleRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_IS_ACTIVE, @P_RULES_JSON)";

            var parameters = new
            {
                P_ID = request.Id,
                P_NAME = request.Name,
                P_IS_ACTIVE = request.IsActive,
                P_RULES_JSON = request.Rules
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao cadastra o roi da câmera");
        }
    }
}
