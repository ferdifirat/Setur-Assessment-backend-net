namespace ReportService.Infrastructure
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
