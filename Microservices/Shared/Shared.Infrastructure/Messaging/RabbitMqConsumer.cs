using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Kernel.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.Messaging;

public class RabbitMqConsumer : IEventConsumer, IDisposable
{
    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private IModel _channel;
    private string _queueName;
    private readonly Action<ReportRequestedEvent> _messageHandler;

    public RabbitMqConsumer(IConfiguration configuration, Action<ReportRequestedEvent> messageHandler)
    {
        _messageHandler = messageHandler;
        
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
        
        // Declare queue
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, "report_exchange", "");
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
            _channel = _persistentConnection.CreateModel();
            _channel.ExchangeDeclare("report_exchange", ExchangeType.Fanout, durable: true);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, "report_exchange", "");
        }

        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            try
            {
                var @event = JsonSerializer.Deserialize<ReportRequestedEvent>(message);
                if (@event != null)
                {
                    _messageHandler(@event);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        
        return Task.CompletedTask;
    }

    public Task StopConsumingAsync()
    {
        _channel?.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _persistentConnection?.Dispose();
    }
} 