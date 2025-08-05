using ContactService.Application;
using ContactService.Commands;
using ContactService.Domain;
using ContactService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Kernel.Results;
using Xunit;

namespace ContactService.Tests;

public class HandlerTests : IDisposable
{
    private readonly ContactDbContext _context;
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<IContactInformationRepository> _contactInformationRepositoryMock;

    public HandlerTests()
    {
        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ContactDbContext(options);
        
        _contactRepositoryMock = new Mock<IContactRepository>();
        _contactInformationRepositoryMock = new Mock<IContactInformationRepository>();
    }

    [Fact]
    public async Task CreateContactHandler_ValidData_ReturnsSuccess()
    {
        // Arrange
        var handler = new CreateContactHandler(_contactRepositoryMock.Object);
        var command = new CreateContactCommand(
            new CreateContactDto { FirstName = "John", LastName = "Doe", Company = "Test Company" },
            "test-user"
        );

        _contactRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Contact c) => c);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _contactRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Contact>()), Times.Once);
    }

    [Fact]
    public async Task CreateContactHandler_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var handler = new CreateContactHandler(_contactRepositoryMock.Object);
        var command = new CreateContactCommand(
            new CreateContactDto { FirstName = "John", LastName = "Doe", Company = "Test Company" },
            "test-user"
        );

        _contactRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("hata oluştu", result.Error);
    }

    [Fact]
    public async Task CreateContactHandler_NullCreatedBy_UsesSystemAsDefault()
    {
        // Arrange
        var handler = new CreateContactHandler(_contactRepositoryMock.Object);
        var command = new CreateContactCommand(
            new CreateContactDto { FirstName = "John", LastName = "Doe", Company = "Test Company" },
            null
        );

        Contact savedContact = null;
        _contactRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .Callback<Contact>(contact => savedContact = contact)
            .ReturnsAsync((Contact c) => c);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("System", savedContact.CreatedBy);
    }

    [Fact]
    public async Task GetContactByIdHandler_ExistingContact_ReturnsContact()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact
        {
            Id = contactId,
            FirstName = "John",
            LastName = "Doe",
            Company = "Test Company",
            ContactInformations = new List<ContactInformation>
            {
                new() { Id = Guid.NewGuid(), Type = ContactInfoType.Phone, Value = "+905551234567" }
            }
        };

        var handler = new GetContactByIdHandler(_contactRepositoryMock.Object);
        var query = new GetContactByIdQuery(contactId);

        _contactRepositoryMock.Setup(x => x.GetByIdWithInformationsAsync(contactId))
            .ReturnsAsync(contact);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(contactId, result.Value.Id);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("Test Company", result.Value.Company);
        Assert.Single(result.Value.ContactInformations);
        Assert.Equal(ContactInfoType.Phone, result.Value.ContactInformations.First().Type);
    }

    [Fact]
    public async Task GetContactByIdHandler_NonExistingContact_ReturnsFailure()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new GetContactByIdHandler(_contactRepositoryMock.Object);
        var query = new GetContactByIdQuery(contactId);

        _contactRepositoryMock.Setup(x => x.GetByIdWithInformationsAsync(contactId))
            .ReturnsAsync((Contact)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bulunamadı", result.Error);
    }

    [Fact]
    public async Task GetContactByIdHandler_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new GetContactByIdHandler(_contactRepositoryMock.Object);
        var query = new GetContactByIdQuery(contactId);

        _contactRepositoryMock.Setup(x => x.GetByIdWithInformationsAsync(contactId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("hata oluştu", result.Error);
    }

    [Fact]
    public async Task GetAllContactsHandler_ReturnsAllContacts()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Company = "Company A", ContactInformations = new List<ContactInformation>() },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Company = "Company B", ContactInformations = new List<ContactInformation>() }
        };

        var handler = new GetAllContactsHandler(_contactRepositoryMock.Object);
        var query = new GetAllContactsQuery();

        _contactRepositoryMock.Setup(x => x.GetActiveWithInformationsAsync())
            .ReturnsAsync(contacts);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("John", result.Value[0].FirstName);
        Assert.Equal("Jane", result.Value[1].FirstName);
    }

    [Fact]
    public async Task GetAllContactsHandler_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var handler = new GetAllContactsHandler(_contactRepositoryMock.Object);
        var query = new GetAllContactsQuery();

        _contactRepositoryMock.Setup(x => x.GetActiveWithInformationsAsync())
            .ReturnsAsync(new List<Contact>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllContactsHandler_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var handler = new GetAllContactsHandler(_contactRepositoryMock.Object);
        var query = new GetAllContactsQuery();

        _contactRepositoryMock.Setup(x => x.GetActiveWithInformationsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("hata oluştu", result.Error);
    }

    [Fact]
    public async Task UpdateContactHandler_ExistingContact_ReturnsSuccess()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var existingContact = new Contact
        {
            Id = contactId,
            FirstName = "Original",
            LastName = "Name",
            Company = "Original Company"
        };

        var handler = new UpdateContactHandler(_contactRepositoryMock.Object);
        var command = new UpdateContactCommand(
            contactId,
            new CreateContactDto { FirstName = "Updated", LastName = "Name", Company = "Updated Company" },
            "test-user"
        );

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync(existingContact);
        _contactRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Contact c) => c);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _contactRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Contact>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContactHandler_NonExistingContact_ReturnsFailure()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new UpdateContactHandler(_contactRepositoryMock.Object);
        var command = new UpdateContactCommand(
            contactId,
            new CreateContactDto { FirstName = "Updated", LastName = "Name", Company = "Updated Company" },
            "test-user"
        );

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync((Contact)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bulunamadı", result.Error);
    }

    [Fact]
    public async Task DeleteContactHandler_ExistingContact_ReturnsSuccess()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var existingContact = new Contact
        {
            Id = contactId,
            FirstName = "ToDelete",
            LastName = "Contact",
            Company = "Test Company"
        };

        var handler = new DeleteContactHandler(_contactRepositoryMock.Object);
        var command = new DeleteContactCommand(contactId, "test-user");

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync(existingContact);
        _contactRepositoryMock.Setup(x => x.SoftDeleteAsync(contactId, "test-user"))
            .Returns(Task.FromResult(0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _contactRepositoryMock.Verify(x => x.SoftDeleteAsync(contactId, "test-user"), Times.Once);
    }

    [Fact]
    public async Task DeleteContactHandler_NonExistingContact_ReturnsFailure()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new DeleteContactHandler(_contactRepositoryMock.Object);
        var command = new DeleteContactCommand(contactId, "test-user");

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync((Contact)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bulunamadı", result.Error);
    }

    [Fact]
    public async Task CreateContactInformationHandler_ValidData_ReturnsSuccess()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Contact { Id = contactId, FirstName = "John", LastName = "Doe" };

        var handler = new CreateContactInformationHandler(_contactInformationRepositoryMock.Object, _contactRepositoryMock.Object);
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = contactId,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            }
        );

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync(contact);
        _contactInformationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ContactInformation>()))
            .ReturnsAsync((ContactInformation ci) => ci);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _contactInformationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ContactInformation>()), Times.Once);
    }

    [Fact]
    public async Task CreateContactInformationHandler_NonExistingContact_ReturnsFailure()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new CreateContactInformationHandler(_contactInformationRepositoryMock.Object, _contactRepositoryMock.Object);
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = contactId,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            }
        );

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync((Contact)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bulunamadı", result.Error);
    }

    [Fact]
    public async Task DeleteContactInformationHandler_ExistingInformation_ReturnsSuccess()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        var contactInfo = new ContactInformation
        {
            Id = infoId,
            Type = ContactInfoType.Phone,
            Value = "+905551234567"
        };

        var handler = new DeleteContactInformationHandler(_contactInformationRepositoryMock.Object);
        var command = new DeleteContactInformationCommand(infoId);

        _contactInformationRepositoryMock.Setup(x => x.GetByIdAsync(infoId))
            .ReturnsAsync(contactInfo);
        _contactInformationRepositoryMock.Setup(x => x.DeleteAsync(infoId))
            .Returns(Task.FromResult(0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _contactInformationRepositoryMock.Verify(x => x.DeleteAsync(infoId), Times.Once);
    }

    [Fact]
    public async Task DeleteContactInformationHandler_NonExistingInformation_ReturnsFailure()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        var handler = new DeleteContactInformationHandler(_contactInformationRepositoryMock.Object);
        var command = new DeleteContactInformationCommand(infoId);

        _contactInformationRepositoryMock.Setup(x => x.GetByIdAsync(infoId))
            .ReturnsAsync((ContactInformation)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("bulunamadı", result.Error);
    }

    [Fact]
    public async Task GetContactInformationByContactIdHandler_ExistingContact_ReturnsInformations()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contactInformations = new List<ContactInformation>
        {
            new() { Id = Guid.NewGuid(), Type = ContactInfoType.Phone, Value = "+905551234567" },
            new() { Id = Guid.NewGuid(), Type = ContactInfoType.Email, Value = "test@example.com" }
        };

        var handler = new GetContactInformationByContactIdHandler(_contactInformationRepositoryMock.Object);
        var query = new GetContactInformationByContactIdQuery(contactId);

        _contactInformationRepositoryMock.Setup(x => x.GetByContactIdAsync(contactId))
            .ReturnsAsync(contactInformations);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(ContactInfoType.Phone, result.Value[0].Type);
        Assert.Equal(ContactInfoType.Email, result.Value[1].Type);
    }

    [Fact]
    public async Task GetContactInformationByContactIdHandler_NoInformations_ReturnsEmptyList()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var handler = new GetContactInformationByContactIdHandler(_contactInformationRepositoryMock.Object);
        var query = new GetContactInformationByContactIdQuery(contactId);

        _contactInformationRepositoryMock.Setup(x => x.GetByContactIdAsync(contactId))
            .ReturnsAsync(new List<ContactInformation>());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}