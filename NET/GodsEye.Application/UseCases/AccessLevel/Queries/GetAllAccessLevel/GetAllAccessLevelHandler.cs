using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.UseCases.AccessLevel.Queries.GetAllAcessLevel;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Queries.GetAllAccessLevel
{
    public class GetAllAccessLevelHandler : IRequestHandler<GetAllAccessLevelRequest, ApiResponse<IEnumerable<AccessLevelModel>>>
    {
        private readonly IDapperContext _context;

        public GetAllAccessLevelHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<AccessLevelModel>>> Handle(GetAllAccessLevelRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_GET_ALL()";

            var parameters = new { };

            var result = await _context.QuerySqlAsync<AccessLevelModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<AccessLevelModel>>.Ok(result);
        }
    }
}
