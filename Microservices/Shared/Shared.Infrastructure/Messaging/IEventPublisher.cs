using Shared.Kernel.Events;

namespace Shared.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishReportRequestedAsync(ReportRequestedEvent @event);
}