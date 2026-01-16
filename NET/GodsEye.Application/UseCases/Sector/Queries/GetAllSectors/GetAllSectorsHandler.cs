using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Queries.GetAllSectors
{
    public class GetAllSectorsHandler : IRequestHandler<GetAllSectorsRequest, ApiResponse<IEnumerable<SectorModel>>>
    {
        private readonly ISectorQueryRepository _sectorQueryRepository;

        public GetAllSectorsHandler(ISectorQueryRepository sectorQueryRepository)
        {
            _sectorQueryRepository = sectorQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<SectorModel>>> Handle(GetAllSectorsRequest request, CancellationToken cancellationToken)
        {
            var cameras = await _sectorQueryRepository.GetAll(cancellationToken);

            return ApiResponse<IEnumerable<SectorModel>>.Ok(cameras);
        }
    }
}
