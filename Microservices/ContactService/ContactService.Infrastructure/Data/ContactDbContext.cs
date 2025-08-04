using ContactService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactService.Infrastructure.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Company).HasMaxLength(200);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.DeletedAt);
                entity.Property(e => e.DeletedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<ContactInformation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ContactId).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Value).IsRequired().HasMaxLength(500);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.DeletedAt);
                entity.Property(e => e.DeletedBy).HasMaxLength(100);

                entity.HasOne(e => e.Contact)
                    .WithMany(c => c.ContactInformations)
                    .HasForeignKey(e => e.ContactId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
