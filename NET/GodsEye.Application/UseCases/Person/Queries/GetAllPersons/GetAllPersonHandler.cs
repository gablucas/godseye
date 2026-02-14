using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersons
{
    internal class GetAllPersonHandler : IRequestHandler<GetAllPersonRequest, ApiResponse<IEnumerable<PersonModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPersonHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<PersonModel>>> Handle(GetAllPersonRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_GET_ALL()";

            var parameters = new { };

            var persons = await _context.QuerySqlAsync<PersonModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<PersonModel>>.Ok(persons);
        }
    }
}
