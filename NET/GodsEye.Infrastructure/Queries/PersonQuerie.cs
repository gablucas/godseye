using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Infrastructure.Queries
{
    public class PersonQuerie : IPersonQueries
    {
        private readonly IDapperContext _context;

        public PersonQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<PersonModel?> GetById(int personId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await _context.QuerySingleSqlAsync<PersonModel>(query, parameters, cancellationToken);
        }
    }
}
