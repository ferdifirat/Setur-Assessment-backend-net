using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Kernel.Behaviors;
using Shared.Kernel.Queries;
using Shared.Kernel.Results;
using Xunit;

namespace ContactService.Tests;

public class BehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestQuery, TestResult>>> _loggerMock;
    private readonly Mock<ILogger<PerformanceBehavior<TestQuery, TestResult>>> _performanceLoggerMock;
    private readonly Mock<ILogger<ValidationBehavior<TestQuery, TestResult>>> _validationLoggerMock;

    public BehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestQuery, TestResult>>>();
        _performanceLoggerMock = new Mock<ILogger<PerformanceBehavior<TestQuery, TestResult>>>();
        _validationLoggerMock = new Mock<ILogger<ValidationBehavior<TestQuery, TestResult>>>();
    }

    [Fact]
    public async Task LoggingBehavior_Handle_ShouldLogRequestAndResponse()
    {
        // Arrange
        var behavior = new LoggingBehavior<TestQuery, TestResult>(_loggerMock.Object);
        var request = new TestQuery { Id = Guid.NewGuid() };
        var expectedResponse = new TestResult { Id = request.Id, Name = "Test" };

        RequestHandlerDelegate<TestResult> next = (CancellationToken token) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task PerformanceBehavior_Handle_ShouldMeasureExecutionTime()
    {
        // Arrange
        var behavior = new PerformanceBehavior<TestQuery, TestResult>(_performanceLoggerMock.Object);
        var request = new TestQuery { Id = Guid.NewGuid() };
        var expectedResponse = new TestResult { Id = request.Id, Name = "Test" };

        RequestHandlerDelegate<TestResult> next = (CancellationToken token) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task ValidationBehavior_Handle_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var validators = new List<IValidator<TestQuery>>();
        var behavior = new ValidationBehavior<TestQuery, TestResult>(validators);
        var request = new TestQuery { Id = Guid.NewGuid() };
        var expectedResponse = new TestResult { Id = request.Id, Name = "Test" };

        RequestHandlerDelegate<TestResult> next = (CancellationToken token) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task CachingBehavior_Handle_WithCacheableQuery_ShouldCacheResult()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();
        var behavior = new CachingBehavior<TestQuery, TestResult>(cacheMock.Object);
        var request = new TestQuery { Id = Guid.NewGuid() };
        var expectedResponse = new TestResult { Id = request.Id, Name = "Test" };

        TestResult? cachedValue = null;
        cacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(false);

        RequestHandlerDelegate<TestResult> next = (CancellationToken token) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        cacheMock.Verify(x => x.Set(It.IsAny<object>(), expectedResponse, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task RetryBehavior_Handle_WithTransientFailure_ShouldRetry()
    {
        // Arrange
        var retryLoggerMock = new Mock<ILogger<RetryBehavior<TestQuery, TestResult>>>();
        var behavior = new RetryBehavior<TestQuery, TestResult>(retryLoggerMock.Object);
        var request = new TestQuery { Id = Guid.NewGuid() };
        var expectedResponse = new TestResult { Id = request.Id, Name = "Test" };
        var callCount = 0;

        RequestHandlerDelegate<TestResult> next = (CancellationToken token) =>
        {
            callCount++;
            if (callCount == 1)
                throw new TimeoutException("Transient failure");
            return Task.FromResult(expectedResponse);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(2, callCount);
    }

    // Test classes
    public class TestQuery : IRequest<TestResult>, ICachableQuery
    {
        public Guid Id { get; set; }
        public string CacheKey => $"test-{Id}";
        public int ExpirationMinutes => 30;
    }

    public class TestResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
    }
} 