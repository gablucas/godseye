using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersonLogs
{
    public sealed record GetAllPersonLogsRequest : IRequest<ApiResponse<IEnumerable<PersonLogModel>>>;
}
