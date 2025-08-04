namespace Shared.Infrastructure.Messaging;

public interface IEventConsumer
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
} 