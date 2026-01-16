using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Queries.GetAllSectors
{
    public sealed record GetAllSectorsRequest() : IRequest<ApiResponse<IEnumerable<SectorModel>>>;
}
