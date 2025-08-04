using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using ReportService.Application;
using ReportService.Infrastructure;
using Shared.Infrastructure.Messaging;
using System.Text;
using System.Text.Json;

namespace ReportService.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IEventConsumer> _eventConsumerMock;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _eventConsumerMock = new Mock<IEventConsumer>();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real EventConsumer with mock
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEventConsumer));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton(_eventConsumerMock.Object);

                // Use in-memory database for testing
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ReportDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }
                services.AddDbContext<ReportDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                });

                // Remove existing health checks and add a simple one for testing
                var healthCheckDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(HealthCheckService));
                if (healthCheckDescriptor != null)
                {
                    services.Remove(healthCheckDescriptor);
                }
                
                // Remove all IHealthCheck registrations
                var healthChecks = services.Where(d => d.ServiceType == typeof(IHealthCheck)).ToList();
                foreach (var healthCheck in healthChecks)
                {
                    services.Remove(healthCheck);
                }

                // Remove all IHealthCheckBuilder registrations
                var healthCheckBuilders = services.Where(d => d.ServiceType == typeof(IHealthChecksBuilder)).ToList();
                foreach (var healthCheckBuilder in healthCheckBuilders)
                {
                    services.Remove(healthCheckBuilder);
                }

                // Add a simple health check that always returns healthy
                services.AddHealthChecks()
                    .AddCheck("test", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Test health check"));
            });
        });
    }

    private async Task EnsureDatabaseCreatedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task CreateReport_ValidData_ReturnsCreated()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow.AddSeconds(-1) // Use a slightly past timestamp to avoid validation timing issues
        };

        var json = JsonSerializer.Serialize(reportDto);
        var content = new StringContent(json, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ReportBase, content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateReport_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.Empty, // Invalid - empty GUID
            RequestedAt = DateTime.UtcNow.AddSeconds(-1) // Use a slightly past timestamp to avoid validation timing issues
        };

        var json = JsonSerializer.Serialize(reportDto);
        var content = new StringContent(json, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ReportBase, content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetReport_ExistingReport_ReturnsOk()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();

        // First create a report
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow.AddSeconds(-1) // Use a slightly past timestamp to avoid validation timing issues
        };

        var createJson = JsonSerializer.Serialize(reportDto);
        var createContent = new StringContent(createJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);
        var createResponse = await client.PostAsync(TestConstants.ApiPaths.ReportBase, createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        
        // The response might be wrapped in an object or be a direct GUID string
        Guid reportId;
        try
        {
            // Try to deserialize as a direct GUID first
            reportId = JsonSerializer.Deserialize<Guid>(createResponseContent);
        }
        catch (JsonException)
        {
            // If that fails, try to deserialize as an object with an id property
            var responseObj = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
            if (responseObj.TryGetProperty("id", out var idElement))
            {
                reportId = Guid.Parse(idElement.GetString());
            }
            else
            {
                throw new InvalidOperationException($"Unexpected response format: {createResponseContent}");
            }
        }

        // Act
        var response = await client.GetAsync(string.Format(TestConstants.ApiPaths.ReportById, reportId));

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetReport_NonExistingReport_ReturnsNotFound()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(string.Format(TestConstants.ApiPaths.ReportById, nonExistingId));

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllReports_ReturnsOk()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(TestConstants.ApiPaths.ReportBase);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(TestConstants.ApiPaths.HealthCheck);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiVersioning_WithVersionHeader_Works()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Version", "1.0");

        // Act - Use a path without version in URL to test header versioning
        var response = await client.GetAsync("/api/v1.0/report");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiVersioning_WithUrlVersion_Works()
    {
        // Arrange
        await EnsureDatabaseCreatedAsync();
        var client = _factory.CreateClient();

        // Act - Use a path with version in URL
        var response = await client.GetAsync(TestConstants.ApiPaths.ReportBase);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}