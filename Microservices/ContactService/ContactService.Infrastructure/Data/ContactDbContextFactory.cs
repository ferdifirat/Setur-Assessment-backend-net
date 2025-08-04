using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContactService.Infrastructure.Data
{
    public class ContactDbContextFactory : IDesignTimeDbContextFactory<ContactDbContext>
    {
        public ContactDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContactDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=ContactServiceDb;Username=postgres;Password=postgres;Port=5432;");

            return new ContactDbContext(optionsBuilder.Options);
        }
    }
} 