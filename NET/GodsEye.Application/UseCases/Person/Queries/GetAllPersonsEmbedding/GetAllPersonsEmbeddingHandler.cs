using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersonEmbedding
{
    public class GetAllPersonEmbeddingHandler : IRequestHandler<GetAllPersonsEmbeddingRequest, ApiResponse<IEnumerable<PersonEmbeddingModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPersonEmbeddingHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<PersonEmbeddingModel>>> Handle(GetAllPersonsEmbeddingRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_GET_ALL_EMBEDDING()";

            var parameters = new { };

            var persons = await _context.QuerySqlAsync<PersonEmbeddingModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<PersonEmbeddingModel>>.Ok(persons);
        }
    }
}
