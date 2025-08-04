using Shared.Kernel.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReportService.Domain
{
    public class Report : BaseEntity
    {
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.Preparing;

        public DateTime? CompletedAt { get; set; }

        public string? LocationStatistics { get; set; } // JSON formatında istatistikler
    }

    public enum ReportStatus
    {
        Preparing,
        Completed
    }
}
