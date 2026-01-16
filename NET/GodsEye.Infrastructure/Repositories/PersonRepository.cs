using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.Json;

namespace GodsEye.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<ProcedureResult> Create(PersonEntity person, CancellationToken cancellationToken)
        {
            var pName = new MySqlParameter("@P_NAME", person.Name);
            var pEmbedding = new MySqlParameter("@P_EMBEDDING", person.Embedding);
            var pImagePath = new MySqlParameter("@P_IMAGE_PATH", person.ImagePath);

            var sectorsJSON = JsonSerializer.Serialize(person.Sectors);

            var pSectors = new MySqlParameter("@P_SECTORS_ID_JSON", MySqlDbType.JSON)
            {
                Value = sectorsJSON
            };


            var result = await _context.ProcedureResult
                .FromSqlRaw(
                    "CALL SP_PERSON_CREATE(@P_NAME, @P_EMBEDDING, @P_IMAGE_PATH, @P_SECTORS_ID_JSON)",
                    pName, pEmbedding, pImagePath, pSectors
                )
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault() ?? new ProcedureResult
            {
                Erro = 1,
                Mensagem = "Houve um ao cadastrar a pessoa",
                Id = 0
            };
        }
    }
}
