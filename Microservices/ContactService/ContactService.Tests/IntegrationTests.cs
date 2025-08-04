using ContactService.Application;
using ContactService.Domain;
using ContactService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Shared.Infrastructure.Messaging;
using Shared.Kernel.Events;
using System.Text;
using System.Text.Json;

namespace ContactService.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IEventPublisher> _eventPublisherMock;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _eventPublisherMock = new Mock<IEventPublisher>();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real EventPublisher with mock
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEventPublisher));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton(_eventPublisherMock.Object);

                // Use in-memory database for testing
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ContactDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }
                services.AddDbContext<ContactDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
    }

    [Fact]
    public async Task CreateContact_ValidData_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var contactDto = new CreateContactDto
        {
            FirstName = TestConstants.TestData.ValidFirstName,
            LastName = TestConstants.TestData.ValidLastName,
            Company = TestConstants.TestData.ValidCompany
        };

        var json = JsonSerializer.Serialize(contactDto);
        var content = new StringContent(json, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ContactBase, content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Ali", responseContent);
    }

    [Fact]
    public async Task CreateContact_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var contactDto = new CreateContactDto
        {
            FirstName = TestConstants.TestData.InvalidFirstName, // Invalid - empty name
            LastName = TestConstants.TestData.ValidLastName,
            Company = TestConstants.TestData.ValidCompany
        };

        var json = JsonSerializer.Serialize(contactDto);
        var content = new StringContent(json, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ContactBase, content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetContact_ExistingContact_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First create a contact
        var contactDto = new CreateContactDto
        {
            FirstName = "Test",
            LastName = "User",
            Company = "Test Company"
        };

        var createJson = JsonSerializer.Serialize(contactDto);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await client.PostAsync("/api/v1.0/contact", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var contactId = JsonSerializer.Deserialize<Guid>(createResponseContent);

        // Act
        var response = await client.GetAsync(string.Format(TestConstants.ApiPaths.ContactById, contactId));

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test", responseContent);
    }

    [Fact]
    public async Task GetContact_NonExistingContact_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1.0/contact/{nonExistingId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(TestConstants.ApiPaths.ContactBase);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddContactInformation_ValidData_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First create a contact
        var contactDto = new CreateContactDto
        {
            FirstName = "Test",
            LastName = "User",
            Company = "Test Company"
        };

        var createJson = JsonSerializer.Serialize(contactDto);
        var createContent = new StringContent(createJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);
        var createResponse = await client.PostAsync(TestConstants.ApiPaths.ContactBase, createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var contactId = JsonSerializer.Deserialize<Guid>(createResponseContent);

        // Add contact information
        var contactInfoDto = new CreateContactInformationDto
        {
            ContactId = contactId,
            Type = ContactInfoType.Phone,
            Value = TestConstants.TestData.ValidPhone
        };

        var infoJson = JsonSerializer.Serialize(contactInfoDto);
        var infoContent = new StringContent(infoJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ContactInformation, infoContent);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddContactInformation_InvalidPhone_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First create a contact
        var contactDto = new CreateContactDto
        {
            FirstName = "Test",
            LastName = "User",
            Company = "Test Company"
        };

        var createJson = JsonSerializer.Serialize(contactDto);
        var createContent = new StringContent(createJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);
        var createResponse = await client.PostAsync(TestConstants.ApiPaths.ContactBase, createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var contactId = JsonSerializer.Deserialize<Guid>(createResponseContent);

        // Add invalid contact information
        var contactInfoDto = new CreateContactInformationDto
        {
            ContactId = contactId,
            Type = ContactInfoType.Phone,
            Value = TestConstants.TestData.InvalidPhone // Invalid phone format
        };

        var infoJson = JsonSerializer.Serialize(contactInfoDto);
        var infoContent = new StringContent(infoJson, Encoding.UTF8, TestConstants.ContentTypes.ApplicationJson);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ContactInformation, infoContent);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RequestReport_ReturnsAccepted()
    {
        // Arrange
        var client = _factory.CreateClient();
        _eventPublisherMock.Setup(x => x.PublishReportRequestedAsync(It.IsAny<ReportRequestedEvent>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await client.PostAsync(TestConstants.ApiPaths.ReportRequest, null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
        _eventPublisherMock.Verify(x => x.PublishReportRequestedAsync(It.IsAny<ReportRequestedEvent>()), Times.Once);
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
        var response = await client.GetAsync(TestConstants.ApiPaths.ContactBase);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}