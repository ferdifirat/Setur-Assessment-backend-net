using ReportService.Domain;
using ReportService.Domain.Repositories;
using Shared.Infrastructure.Repositories;

namespace ReportService.Infrastructure.Repositories
{
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        private readonly ReportDbContext _context;

        public ReportRepository(ReportDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
