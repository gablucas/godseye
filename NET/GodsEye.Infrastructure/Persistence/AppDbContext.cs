using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ENTITIES
        public DbSet<PersonEntity> Person { get; set; }
        public DbSet<CameraEntity> Camera { get; set; }
        public DbSet<SectorEntity> Sector { get; set; }


        // DTOS
        public DbSet<ProcedureResult> ProcedureResult { get; set; }
        public DbSet<CameraModel> CameraModel { get; set; }
        public DbSet<PersonModel> PersonModel { get; set; }
        public DbSet<SectorModel> SectorModel { get; set; }
        public DbSet<CameraConnectionModel> CameraConnectionModel { get; set; }
        public DbSet<PersonEmbeddingModel> PersonEmbeddingModel { get; set; }
        public DbSet<PersonLogModel> PersonLogModel { get; set; }
        public DbSet<CameraLogModel> CameraLogModel { get; set; }
        public DbSet<MonitoringDataModel> MonitoringDataModel { get; set; }
        public DbSet<EnvironmentMonitoringModel> EnvironmentMonitoringModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProcedureResult>().HasNoKey().ToView(null);

            modelBuilder.Entity<CameraModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PersonModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<SectorModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<CameraConnectionModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PersonEmbeddingModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<PersonLogModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<CameraLogModel>().HasNoKey().ToView(null);

            modelBuilder.Entity<MonitoringDataModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<EnvironmentMonitoringModel>().HasNoKey().ToView(null);

        }
    }
}
