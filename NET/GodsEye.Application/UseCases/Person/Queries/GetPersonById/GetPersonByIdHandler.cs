using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetPersonById
{
    public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdRequest, ApiResponse<PersonModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetPersonByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PersonModel>> Handle(GetPersonByIdRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new Dictionary<string, object?>
            {
                ["@P_PERSON_ID"] = request.personId,
            };

            var result = await _context.QuerySingleSqlAsync<PersonModel>(query, parameters, cancellationToken);
            return ApiResponse<PersonModel>.Ok(result);
        }
    }
}
