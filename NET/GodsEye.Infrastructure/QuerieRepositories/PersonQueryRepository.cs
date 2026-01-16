using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class PersonQueryRepository : IPersonQueryRepository
    {
        private readonly AppDbContext _context;

        public PersonQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.PersonModel
                .FromSqlRaw("CALL SP_PERSON_GET_ALL()")
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<PersonEmbeddingModel>> GetAllEmbeddings(CancellationToken cancellationToken)
        {
            var result = await _context.PersonEmbeddingModel
                .FromSqlRaw("CALL SP_PERSON_GET_ALL_EMBEDDING()")
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<PersonLogModel>> GetLogsByPersonId(int personId, CancellationToken cancellationToken)
        {
            var pPersonId = new MySqlParameter("@P_PERSON_ID", personId);

            var result = await _context.PersonLogModel
                .FromSqlRaw("CALL SP_PERSON_GET_ENVIRONMENT_MONITORING_LOG(@P_PERSON_ID)", pPersonId)
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<IEnumerable<PersonLogModel>> GetAllPersonLogs(CancellationToken cancellationToken) 
        {

            var result = await _context.PersonLogModel
                .FromSqlRaw("CALL SP_PERSON_GET_ALL_ENVIRONMENT_MONITORING_LOG()")
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
