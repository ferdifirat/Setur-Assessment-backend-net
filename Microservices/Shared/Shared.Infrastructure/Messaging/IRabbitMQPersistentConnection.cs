using RabbitMQ.Client;

namespace Shared.Infrastructure.Messaging;

public interface IRabbitMQPersistentConnection : IDisposable
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
} 