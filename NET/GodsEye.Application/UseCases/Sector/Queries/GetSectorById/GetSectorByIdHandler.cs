using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Queries.GetSectorById
{
    public class GetSectorByIdHandler : IRequestHandler<GetSectorByIdRequest, ApiResponse<SectorModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetSectorByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<SectorModel>> Handle(GetSectorByIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_GET_BY_ID(@P_SECTOR_ID)";

            var parameters = new
            {
                P_SECTOR_ID = request.SectorId,
            };

            var result = await _context.QuerySingleSqlAsync<SectorModel>(sql, parameters, cancellationToken);
            return ApiResponse<SectorModel>.Ok(result);
        }
    }
}
