using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Commands.CreateSector
{
    public sealed record CreateSectorRequest : IRequest<ApiResponse<int>>
    {
        public string Name { get; set; }
        public IEnumerable<int> NotificationGroups { get; set; } = new List<int>();
    }
}
