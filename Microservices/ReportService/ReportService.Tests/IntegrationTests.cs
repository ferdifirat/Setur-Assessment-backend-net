using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
                });
            });
        });
    }

    [Fact]
    public async Task CreateReport_ValidData_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow
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
        var client = _factory.CreateClient();
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.Empty, // Invalid - empty GUID
            RequestedAt = DateTime.UtcNow
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
        var client = _factory.CreateClient();

        // First create a report
        var reportDto = new CreateReportDto
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow
        };

        var createJson = JsonSerializer.Serialize(reportDto);
        var createContent = new StringContent(createJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);
        var createResponse = await client.PostAsync(TestConstants.ApiPaths.ReportBase, createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var reportId = JsonSerializer.Deserialize<Guid>(createResponseContent);

        // Act
        var response = await client.GetAsync(string.Format(TestConstants.ApiPaths.ReportById, reportId));

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetReport_NonExistingReport_ReturnsNotFound()
    {
        // Arrange
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
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Version", "1.0");

        // Act
        var response = await client.GetAsync(TestConstants.ApiPaths.ReportBase);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}