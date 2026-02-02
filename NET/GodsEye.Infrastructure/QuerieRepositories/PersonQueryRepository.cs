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

        public async Task<PersonModel?> GetById(int personId, CancellationToken cancellationToken)
        {
            var pPersonId = new MySqlParameter("@P_PERSON_ID", personId);

            var result = await _context.PersonModel
                .FromSqlRaw("CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)", personId)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<PersonEmbeddingModel>> GetAllEmbeddings(CancellationToken cancellationToken)
        {
            var result = await _context.PersonEmbeddingModel
                .FromSqlRaw("CALL SP_PERSON_GET_ALL_EMBEDDING()")
                .ToListAsync();

            return result;
        }
    }
}
