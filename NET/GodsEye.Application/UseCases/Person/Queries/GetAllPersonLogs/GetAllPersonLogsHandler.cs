using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersonLogs
{
    public class GetAllPersonLogsHandler : IRequestHandler<GetAllPersonLogsRequest, ApiResponse<IEnumerable<PersonLogModel>>>
    {
        private readonly IPersonQueryRepository _personQueryRepository;

        public GetAllPersonLogsHandler(IPersonQueryRepository personQueryRepository)
        {
            _personQueryRepository = personQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<PersonLogModel>>> Handle(GetAllPersonLogsRequest request, CancellationToken cancellationToken)
        {
            var persons = await _personQueryRepository.GetAllPersonLogs(cancellationToken);

            return ApiResponse<IEnumerable<PersonLogModel>>.Ok(persons);
        }
    }
}
