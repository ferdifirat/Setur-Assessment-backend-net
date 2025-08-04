using Microsoft.EntityFrameworkCore;
using ReportService.Domain;

namespace ReportService.Infrastructure
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        {
        }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CompletedAt);
                entity.Property(e => e.LocationStatistics);

                // BaseEntity properties
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.DeletedAt);
                entity.Property(e => e.DeletedBy).HasMaxLength(100);
            });
        }
    }
}
