using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Routine.Queries
{

    public sealed record GetAllRoutinesRequest() : IRequest<IEnumerable<RoutineModel>>;

    public class GetAllRoutinesHandler : IRequestHandler<GetAllRoutinesRequest, IEnumerable<RoutineModel>>
    {
        private readonly IDapperContext _context;

        public GetAllRoutinesHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoutineModel>> Handle(GetAllRoutinesRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_GET_ALL()";
            var result = await _context.QuerySqlAsync<RoutineModel>(sql, new { }, cancellationToken);

            return result;
        }
    }
}
