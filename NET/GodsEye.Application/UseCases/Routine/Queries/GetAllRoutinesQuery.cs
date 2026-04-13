using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.Routine.Queries
{

    public sealed record GetAllRoutinesRequest() : IRequest<ApiResponse<IEnumerable<RoutineModel>>>;

    public class GetAllRoutinesHandler : IRequestHandler<GetAllRoutinesRequest, ApiResponse<IEnumerable<RoutineModel>>>
    {
        private readonly IDapperContext _context;

        public GetAllRoutinesHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<RoutineModel>>> Handle(GetAllRoutinesRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_GET_ALL()";
            var result = await _context.QuerySqlAsync<RoutineModel>(sql, new { }, cancellationToken);

            return ApiResponse<IEnumerable<RoutineModel>>.Ok(result);
        }
    }
}
