using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.Routine.Queries
{
    public class GetAllRoutineResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoutineRuleTypeEnum Type { get; set; }
    }

    public sealed record GetAllRoutinesRequest() : IRequest<ApiResponse<IEnumerable<GetAllRoutineResponse>>>;

    public class GetAllRoutinesHandler : IRequestHandler<GetAllRoutinesRequest, ApiResponse<IEnumerable<GetAllRoutineResponse>>>
    {
        private readonly IDapperContext _context;

        public GetAllRoutinesHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<GetAllRoutineResponse>>> Handle(GetAllRoutinesRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_GET_ALL()";
            var result = await _context.QuerySqlAsync<GetAllRoutineResponse>(sql, new { }, cancellationToken);

            return ApiResponse<IEnumerable<GetAllRoutineResponse>>.Ok(result);
        }
    }
}
