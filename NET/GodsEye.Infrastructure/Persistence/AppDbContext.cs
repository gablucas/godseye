using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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


        // DTOS
        public DbSet<ProcedureResult> ProcedureResult { get; set; }
        public DbSet<CameraModel> CameraModel { get; set; }
        public DbSet<PersonModel> PersonModel { get; set; }
        public DbSet<SectorModel> SectorModel { get; set; }
        public DbSet<CameraConnectionModel> CameraConnectionModel { get; set; }
        public DbSet<PersonEmbeddingModel> PersonEmbeddingModel { get; set; }
        public DbSet<CameraLogModel> CameraLogModel { get; set; }
        public DbSet<CameraByFeatureModel> CameraByFeatureModel { get; set; }
        public DbSet<MonitoringDataModel> MonitoringDataModel { get; set; }
        public DbSet<EnvironmentMonitoringModel> EnvironmentMonitoringModel { get; set; }
        public DbSet<IncidentRecordingModel> IncidentRecordingModel { get; set; }
        public DbSet<IncidentRecordingProcessModel> IncidentRecordingProcessModel { get; set; }
        public DbSet<DwellTimeMonitoringModel> DwellTimeMonitoringModel { get; set; }
        public DbSet<DwellTimeMonitoringDetailsModel> DwellTimeMonitoringDetailsModel { get; set; }
        public DbSet<CameraFeatureModel> CameraFeatureModel { get; set; }
        public DbSet<NotificationGroupModel> NotificationGroupModel { get; set; }
        public DbSet<EnvironmentMonitoringPersonModel> EnvironmentMonitoringPersonModel { get; set; }
        public DbSet<EnvironmentMonitoringSectorModel> EnvironmentMonitoringSectorModel { get; set; }

        public async Task<int> ExecuteSqlAsync(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken)
        {
            var sqlParameters = parameters
            .Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value))
            .ToArray();

            return await Database.ExecuteSqlRawAsync(sql, sqlParameters, cancellationToken);
        }

        public async Task<T?> QuerySingleSqlAsync<T>(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken) where T : class
        {
            var sqlParameters = parameters
            .Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value))
            .ToArray();

            var result = await Set<T>()
            .FromSqlRaw(sql, sqlParameters)
            .AsNoTracking()
            .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, CancellationToken cancellationToken) where T : class
        {
            return await Set<T>()
            .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken) where T : class
        {
            var sqlParameters = parameters
            .Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value))
            .ToArray();

            return await Set<T>()
            .FromSqlRaw(sql, sqlParameters)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProcedureResult>().HasNoKey().ToView(null);

            modelBuilder.Entity<CameraModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PersonModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SectorModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<CameraConnectionModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PersonEmbeddingModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<CameraLogModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<CameraByFeatureModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<MonitoringDataModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<EnvironmentMonitoringModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<IncidentRecordingModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<IncidentRecordingProcessModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<DwellTimeMonitoringModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<DwellTimeMonitoringDetailsModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<CameraFeatureModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<NotificationGroupModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<EnvironmentMonitoringPersonModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<EnvironmentMonitoringSectorModel>().HasNoKey().ToView(null);
        }
    }
}
