using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.Queries;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetPersonById
{
    public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdRequest, ApiResponse<PersonModel>>
    {
        private readonly IPersonQueries _personQuerie;

        public GetPersonByIdHandler(IPersonQueries personQuerie)
        {
            _personQuerie = personQuerie;
        }

        public async Task<ApiResponse<PersonModel>> Handle(GetPersonByIdRequest request, CancellationToken cancellationToken)
        {
            var person = await _personQuerie.GetById(request.personId, cancellationToken);

            if (person is null)
                return ApiResponse<PersonModel>.Fail(404, "Pessoa não encontrada");

            return ApiResponse<PersonModel>.Ok(person);
        }
    }
}
