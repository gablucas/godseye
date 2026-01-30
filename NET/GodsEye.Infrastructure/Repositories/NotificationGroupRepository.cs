using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.Json;

namespace GodsEye.Infrastructure.Repositories
{
    public class NotificationGroupRepository : INotificationGroupRepository
    {
        private readonly AppDbContext _context;

        public NotificationGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult> Create(NotificationGroupEntity entity, CancellationToken cancellationToken)
        {
            var pName = new MySqlParameter("@P_NAME", entity.Name);

            var emailsJSON = JsonSerializer.Serialize(entity.Emails);

            var pEmailsJSON = new MySqlParameter("@P_EMAILS_JSON", MySqlDbType.JSON)
            {
                Value = emailsJSON
            };

            var result = await _context.ProcedureResult
                .FromSqlRaw("CALL SP_NOTIFICATION_GROUP_CREATE(@P_NAME, @P_EMAILS_JSON)", pName, pEmailsJSON)
                .ToListAsync();

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure para cadastrar os emails!");
        }
    }
}
