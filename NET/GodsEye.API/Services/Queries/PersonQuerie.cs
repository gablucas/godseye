using GodsEye.API.DTO;
using GodsEye.API.Interfaces;

namespace GodsEye.API.Services.Queries
{
    public class PersonQuerie : IPersonQuerie
    {
        private readonly IDapperContext _context;

        public PersonQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<PersonDTO?> GetById(int personId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await _context.QuerySingleSqlAsync<PersonDTO>(query, parameters, cancellationToken);
        }

        public async Task<IEnumerable<PersonCache>> GetAllCache(CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_ALL_CACHE()";

            var parameters = new {};

            return await _context.QuerySqlAsync<PersonCache>(query, parameters, cancellationToken);
        }
    }
}
