namespace Shared.Kernel.Events;

public class ReportRequestedEvent
{
    public Guid ReportId { get; set; }
    public DateTime RequestedAt { get; set; }
} 