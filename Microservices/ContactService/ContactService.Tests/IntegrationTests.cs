using ContactService.Application;
using ContactService.Domain;
using ContactService.Api.Properties;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Shared.Infrastructure.Messaging;
using Shared.Kernel.Events;
using ContactService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ContactService.Tests
{
    public class IntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _factory;
        private readonly HttpClient _client;

        public IntegrationTests(TestFixture factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateContact_ValidData_ReturnsCreated()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/contacts", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(contact);
            Assert.Equal("John", contact.FirstName);
            Assert.Equal("Doe", contact.LastName);
            Assert.Equal("Test Company", contact.Company);
        }

        [Fact]
        public async Task CreateContact_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "",
                LastName = "",
                Company = ""
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/contacts", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetContactById_ExistingContact_ReturnsOk()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            // Act
            var response = await _client.GetAsync($"/api/v1.0/contacts/{contactId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var retrievedContact = JsonSerializer.Deserialize<ContactDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal("John", retrievedContact.FirstName);
            Assert.Equal("Doe", retrievedContact.LastName);
        }

        [Fact]
        public async Task GetContactById_NonExistingContact_ReturnsNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/v1.0/contacts/{nonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllContacts_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/v1.0/contacts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateContact_ExistingContact_ReturnsOk()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            var updateData = new
            {
                FirstName = "Jane",
                LastName = "Smith",
                Company = "Updated Company"
            };
            var updateJson = JsonSerializer.Serialize(updateData);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/v1.0/contacts/{contactId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteContact_ExistingContact_ReturnsNoContent()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            // Act
            var response = await _client.DeleteAsync($"/api/v1.0/contacts/{contactId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task AddContactInformation_ValidData_ReturnsCreated()
        {
            // Test different URL patterns to see which one works
            var testUrls = new[]
            {
                "/api/v1.0/contacts",
                "/api/contacts",
                "/contacts",
                "/api/v1/contacts"
            };

            foreach (var url in testUrls)
            {
                Console.WriteLine($"Testing URL: {url}");
                var response1 = await _client.GetAsync(url);
                Console.WriteLine($"Response status: {response1.StatusCode}");
            }

            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            
            // Debug: Check what the response actually contains
            Console.WriteLine($"Create contact response status: {createResponse.StatusCode}");
            Console.WriteLine($"Create contact response content: '{createContent}'");
            Console.WriteLine($"Create contact response content length: {createContent?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(createContent))
            {
                Assert.True(false, "Create contact returned empty response body");
                return;
            }
            
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            var contactInfoData = new
            {
                ContactId = contactId,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            };
            var infoJson = JsonSerializer.Serialize(contactInfoData);
            var infoContent = new StringContent(infoJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/contacts/information", infoContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteContactInformation_ExistingInformation_ReturnsNoContent()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            var contactInfoData = new
            {
                ContactId = contactId,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            };
            var infoJson = JsonSerializer.Serialize(contactInfoData);
            var infoContent = new StringContent(infoJson, Encoding.UTF8, "application/json");

            var createInfoResponse = await _client.PostAsync("/api/v1.0/contacts/information", infoContent);
            var createInfoContent = await createInfoResponse.Content.ReadAsStringAsync();
            var contactInfo = JsonSerializer.Deserialize<ContactInformationDto>(createInfoContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var infoId = contactInfo.Id;

            // Act
            var response = await _client.DeleteAsync($"/api/v1.0/contacts/information/{infoId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetContactInformation_ByContactId_ReturnsOk()
        {
            // Arrange
            var contactData = new
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            };
            var json = JsonSerializer.Serialize(contactData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/v1.0/contacts", content);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var contact = JsonSerializer.Deserialize<ContactDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var contactId = contact.Id;

            var contactInfoData = new
            {
                ContactId = contactId,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            };
            var infoJson = JsonSerializer.Serialize(contactInfoData);
            var infoContent = new StringContent(infoJson, Encoding.UTF8, "application/json");

            await _client.PostAsync("/api/v1.0/contacts/information", infoContent);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/contacts/{contactId}/information");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("+905551234567", responseContent);
        }

        [Fact]
        public async Task RequestReport_ReturnsAccepted()
        {
            // Act
            var response = await _client.PostAsync("/api/v1.0/contacts/reports/request", null);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Debug_Routing_Test()
        {
            // Test basic routing
            var response = await _client.GetAsync("/");
            Console.WriteLine($"Root path response: {response.StatusCode}");
            
            // Test if any controller is registered
            var response2 = await _client.GetAsync("/api");
            Console.WriteLine($"/api response: {response2.StatusCode}");
            
            // Test with different version formats
            var response3 = await _client.GetAsync("/api/v1/contacts");
            Console.WriteLine($"/api/v1/contacts response: {response3.StatusCode}");
            
            var response4 = await _client.GetAsync("/api/v1.0/contacts");
            Console.WriteLine($"/api/v1.0/contacts response: {response4.StatusCode}");
            
            // Test health endpoint
            var response5 = await _client.GetAsync("/health");
            Console.WriteLine($"/health response: {response5.StatusCode}");
        }
    }

    public class TestFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;

        public TestFixture()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithDatabase("contactservice_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
            await base.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all RabbitMQ-related services
                var servicesToRemove = services.Where(s => 
                    s.ServiceType == typeof(IRabbitMQPersistentConnection) ||
                    s.ServiceType == typeof(IEventConsumer) ||
                    s.ImplementationType?.Name.Contains("RabbitMQ") == true ||
                    s.ImplementationType?.Name.Contains("RabbitMq") == true).ToList();

                foreach (var service in servicesToRemove)
                {
                    services.Remove(service);
                }

                // Replace IEventPublisher with mock
                var eventPublisherService = services.FirstOrDefault(s => s.ServiceType == typeof(IEventPublisher));
                if (eventPublisherService != null)
                {
                    services.Remove(eventPublisherService);
                }
                services.AddScoped<IEventPublisher, MockEventPublisher>();

                // Configure PostgreSQL for testing using Testcontainers
                var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ContactDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<ContactDbContext>(options =>
                {
                    options.UseNpgsql(_postgresContainer.GetConnectionString());
                });
            });

            builder.Configure(app =>
            {
                // Ensure database is created for tests
                using var scope = app.ApplicationServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
                dbContext.Database.EnsureCreated();
            });

            builder.UseEnvironment("Test");
        }
    }

    public class MockEventPublisher : IEventPublisher
    {
        public Task PublishReportRequestedAsync(ReportRequestedEvent @event)
        {
            // Mock implementation - do nothing for tests
            return Task.CompletedTask;
        }
    }
} 