using Dapper;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ENTITIES
        public DbSet<PersonEntity> Person { get; set; }
        public DbSet<CameraEntity> Camera { get; set; }
        public DbSet<SectorEntity> Sector { get; set; }
        public DbSet<FeatureEntity> Feature { get; set; }
        public DbSet<DwellTimeMonitoringEntity> DwellTimeMonitoring { get; set; }
        public DbSet<NotificationGroupEntity> NotificationGroup { get; set; }

        public async Task<int> ExecuteSqlAsync(string sql, object? parameters, CancellationToken cancellationToken)
        {
            // Pega a conexão do próprio EF Core
            var connection = Database.GetDbConnection();

            // Definição do comando para suportar Cancelamento
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            return await connection.ExecuteAsync(command);
        }

        public async Task<T?> QuerySingleSqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken) where T : class
        {
            var connection = Database.GetDbConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            // QueryFirstOrDefaultAsync substitui toda aquela lógica de .ToListAsync().FirstOrDefault()
            return await connection.QueryFirstOrDefaultAsync<T>(command);
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, CancellationToken cancellationToken) where T : class
        {
            // Reutiliza a lógica passando null nos parametros
            return await QuerySqlAsync<T>(sql, null, cancellationToken);
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken) where T : class
        {
            var connection = Database.GetDbConnection();
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<T>(command);
            return result.ToList();
        }


        // Se quiser manter o delete separado, ok, mas o ExecuteSqlAsync já faria isso.
        public async Task<int> ExecuteDeleteAsync(string sql, object? parameters, CancellationToken cancellationToken)
        {
            return await ExecuteSqlAsync(sql, parameters, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
