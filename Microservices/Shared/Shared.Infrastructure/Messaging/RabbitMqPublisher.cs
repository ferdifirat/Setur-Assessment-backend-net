using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared.Kernel.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.Messaging;

public class RabbitMqPublisher : IEventPublisher, IDisposable
{
    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private IModel _channel;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:User"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
        };

        _persistentConnection = new DefaultRabbitMQPersistentConnection(factory);
        
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        
        _channel = _persistentConnection.CreateModel();
        
        // Declare exchange
        _channel.ExchangeDeclare("report_exchange", ExchangeType.Fanout, durable: true);
    }

    public Task PublishReportRequestedAsync(ReportRequestedEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
            _channel = _persistentConnection.CreateModel();
            _channel.ExchangeDeclare("report_exchange", ExchangeType.Fanout, durable: true);
        }

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "report_exchange",
            routingKey: "",
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _persistentConnection?.Dispose();
    }
}