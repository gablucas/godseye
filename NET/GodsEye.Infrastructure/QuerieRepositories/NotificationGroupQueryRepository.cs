using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class NotificationGroupQueryRepository : INotificationGroupQueryRepository
    {
        private readonly AppDbContext _context;

        public NotificationGroupQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationGroupModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.NotificationGroupModel
                .FromSqlRaw("CALL SP_NOTIFICATION_GROUP_GET_ALL()")
                .ToListAsync();

            return result;
        }
    }
}
