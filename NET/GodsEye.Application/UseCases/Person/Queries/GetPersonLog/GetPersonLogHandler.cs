using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetPersonLog
{
    public class GetPersonLogHandler : IRequestHandler<GetPersonLogRequest, ApiResponse<IEnumerable<PersonLogModel>>>
    {
        private readonly IPersonQueryRepository _personQueryRepository;

        public GetPersonLogHandler(IPersonQueryRepository personQueryRepository)
        {
            _personQueryRepository = personQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<PersonLogModel>>> Handle(GetPersonLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _personQueryRepository.GetLogsByPersonId(request.personId, cancellationToken);
            return ApiResponse<IEnumerable<PersonLogModel>>.Ok(result);
        }
    }
}
