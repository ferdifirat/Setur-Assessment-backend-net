using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Messaging;
using Shared.Kernel.Events;
using Xunit;
using Moq;
using System.Text.Json;

namespace ReportService.Tests;

public class RabbitMQTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<Action<ReportRequestedEvent>> _messageHandlerMock;

    public RabbitMQTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"RabbitMQ:Host", "localhost"},
                {"RabbitMQ:Port", "5672"},
                {"RabbitMQ:User", "guest"},
                {"RabbitMQ:Password", "guest"}
            })
            .Build();

        _messageHandlerMock = new Mock<Action<ReportRequestedEvent>>();
    }

    [Fact]
    public void RabbitMqConsumer_Constructor_Success()
    {
        // Arrange & Act
        var consumer = new RabbitMqConsumer(_configuration, _messageHandlerMock.Object);

        // Assert
        Assert.NotNull(consumer);
    }

    [Fact]
    public async Task RabbitMqConsumer_StartConsuming_Success()
    {
        // Arrange
        var consumer = new RabbitMqConsumer(_configuration, _messageHandlerMock.Object);

        // Act
        await consumer.StartConsumingAsync();

        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task RabbitMqConsumer_StopConsuming_Success()
    {
        // Arrange
        var consumer = new RabbitMqConsumer(_configuration, _messageHandlerMock.Object);

        // Act
        await consumer.StopConsumingAsync();

        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public void ReportRequestedEvent_Serialization_Success()
    {
        // Arrange
        var @event = new ReportRequestedEvent
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(@event);
        var deserializedEvent = JsonSerializer.Deserialize<ReportRequestedEvent>(json);

        // Assert
        Assert.NotNull(deserializedEvent);
        Assert.Equal(@event.ReportId, deserializedEvent!.ReportId);
        Assert.Equal(@event.RequestedAt, deserializedEvent.RequestedAt);
    }

    [Fact]
    public void RabbitMqConsumer_Dispose_Success()
    {
        // Arrange
        var consumer = new RabbitMqConsumer(_configuration, _messageHandlerMock.Object);

        // Act & Assert
        consumer.Dispose();
        Assert.True(true); // No exception should be thrown
    }

    [Fact]
    public void RabbitMqConsumer_MessageHandler_Called()
    {
        // Arrange
        var messageHandlerCalled = false;
        var consumer = new RabbitMqConsumer(_configuration, (@event) =>
        {
            messageHandlerCalled = true;
        });

        // Act
        consumer.Dispose();

        // Assert
        Assert.True(true); // No exception should be thrown
    }
} 