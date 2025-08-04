using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReportService.Infrastructure
{
    public class ReportDbContextFactory : IDesignTimeDbContextFactory<ReportDbContext>
    {
        public ReportDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ReportDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=ReportServiceDb;Username=postgres;Password=postgres;Port=5432;");

            return new ReportDbContext(optionsBuilder.Options);
        }
    }
} 