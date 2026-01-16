using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersons
{
    internal class GetAllPersonHandler : IRequestHandler<GetAllPersonRequest, ApiResponse<IEnumerable<PersonModel>>>
    {
        private readonly IPersonQueryRepository _personQueryRepository;

        public GetAllPersonHandler(IPersonQueryRepository personQUeryRepository)
        {
            _personQueryRepository = personQUeryRepository;
        }

        public async Task<ApiResponse<IEnumerable<PersonModel>>> Handle(GetAllPersonRequest request, CancellationToken cancellationToken)
        {
            var persons = await _personQueryRepository.GetAll(cancellationToken);

            return ApiResponse<IEnumerable<PersonModel>>.Ok(persons);
        }
    }
}
