using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Queries.GetAllSectors
{
    public class GetAllSectorsHandler : IRequestHandler<GetAllSectorsRequest, ApiResponse<IEnumerable<SectorModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllSectorsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<SectorModel>>> Handle(GetAllSectorsRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_GET_ALL()";

            var result = await _context.QuerySqlAsync<SectorModel>(sql, cancellationToken);

            return ApiResponse<IEnumerable<SectorModel>>.Ok(result);
        }
    }
}
