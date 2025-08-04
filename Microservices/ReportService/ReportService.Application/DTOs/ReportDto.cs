using ReportService.Domain;

namespace ReportService.Application
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? LocationStatistics { get; set; }
    }

    public class CreateReportDto
    {
        public Guid ReportId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
