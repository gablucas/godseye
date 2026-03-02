using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Queries.GetAccessLevelById
{
    public class GetAccessLevelByIdHandler : IRequestHandler<GetAccessLevelByIdRequest, ApiResponse<AccessLevelModel>>
    {
        private readonly IDapperContext _context;

        public GetAccessLevelByIdHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AccessLevelModel>> Handle(GetAccessLevelByIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_LEVEL_GET_BY_ID(@P_ACCESS_LEVEL_ID)";

            var parameters = new
            {
                P_ACCESS_LEVEL_ID = request.AccessLevelId
            };

            var result = await _context.QuerySingleSqlAsync<AccessLevelModel>(sql, parameters, cancellationToken);

            return ApiResponse<AccessLevelModel>.Ok(result);
        }
    }
}
