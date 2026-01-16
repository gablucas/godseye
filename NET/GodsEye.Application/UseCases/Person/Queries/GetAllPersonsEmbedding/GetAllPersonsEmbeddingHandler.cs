using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersonEmbedding
{
    public class GetAllPersonEmbeddingHandler : IRequestHandler<GetAllPersonsEmbeddingRequest, ApiResponse<IEnumerable<PersonEmbeddingModel>>>
    {
        private readonly IPersonQueryRepository _personQueryRepository;

        public GetAllPersonEmbeddingHandler(IPersonQueryRepository personQueryRepository)
        {
            _personQueryRepository = personQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<PersonEmbeddingModel>>> Handle(GetAllPersonsEmbeddingRequest request, CancellationToken cancellationToken)
        {
            var result = await _personQueryRepository.GetAllEmbeddings(cancellationToken);

            return ApiResponse<IEnumerable<PersonEmbeddingModel>>.Ok(result);
        }
    }
}
