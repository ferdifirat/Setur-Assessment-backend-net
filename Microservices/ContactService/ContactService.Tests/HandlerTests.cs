using ContactService.Application;
using ContactService.Commands;
using ContactService.Domain;
using Moq;
using Shared.Kernel.Results;

namespace ContactService.Tests;

public class HandlerTests
{
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<IContactInformationRepository> _contactInfoRepositoryMock;

    public HandlerTests()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _contactInfoRepositoryMock = new Mock<IContactInformationRepository>();
    }

    [Fact]
    public async Task CreateContactHandler_ValidContact_ReturnsSuccess()
    {
        // Arrange
        var handler = new CreateContactHandler(_contactRepositoryMock.Object);
        var command = new CreateContactCommand(
            new CreateContactDto { FirstName = "Ali", LastName = "Veli", Company = "ABC" },
            "test-user"
        );

        var contactId = Guid.NewGuid();
        var contact = new Contact
        {
            Id = contactId,
            FirstName = "Ali",
            LastName = "Veli",
            Company = "ABC"
        };
        _contactRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(contact);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contactId, result.Value);
        _contactRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Contact>()), Times.Once);
    }

    [Fact]
    public async Task CreateContactHandler_InvalidContact_ReturnsFailure()
    {
        // Arrange
        var handler = new CreateContactHandler(_contactRepositoryMock.Object);
        var command = new CreateContactCommand(
            new CreateContactDto { FirstName = "", LastName = "Veli" },
            "test-user"
        );

        _contactRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ThrowsAsync(new InvalidOperationException("Validation failed"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Error);
    }

    [Fact]
    public async Task GetContactByIdHandler_ExistingContact_ReturnsSuccess()
    {
        // Arrange
        var handler = new GetContactByIdHandler(_contactRepositoryMock.Object);
        var contactId = Guid.NewGuid();
        var query = new GetContactByIdQuery(contactId);

        var contact = new Contact
        {
            Id = contactId,
            FirstName = "Ali",
            LastName = "Veli",
            Company = "ABC"
        };

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync(contact);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contactId, result.Value.Id);
        Assert.Equal("Ali", result.Value.FirstName);
    }

    [Fact]
    public async Task GetContactByIdHandler_NonExistingContact_ReturnsFailure()
    {
        // Arrange
        var handler = new GetContactByIdHandler(_contactRepositoryMock.Object);
        var contactId = Guid.NewGuid();
        var query = new GetContactByIdQuery(contactId);

        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync((Contact?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Kişi bulunamadı", result.Error);
    }

    [Fact]
    public async Task GetAllContactsHandler_ReturnsSuccess()
    {
        // Arrange
        var handler = new GetAllContactsHandler(_contactRepositoryMock.Object);
        var query = new GetAllContactsQuery();

        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Ali", LastName = "Veli" },
            new() { Id = Guid.NewGuid(), FirstName = "Ayşe", LastName = "Fatma" }
        };

        _contactRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(contacts);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task CreateContactInformationHandler_ValidInformation_ReturnsSuccess()
    {
        // Arrange
        var handler = new CreateContactInformationHandler(_contactInfoRepositoryMock.Object, _contactRepositoryMock.Object);
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Phone,
                Value = "+90 555 123 4567"
            }
        );

        var contactId = command.Dto.ContactId;
        var contact = new Contact { Id = contactId, FirstName = "Test", LastName = "User" };
        
        _contactRepositoryMock.Setup(x => x.GetByIdAsync(contactId))
            .ReturnsAsync(contact);

        var infoId = Guid.NewGuid();
        var contactInfo = new ContactInformation 
        { 
            Id = infoId, 
            Type = ContactInfoType.Phone, 
            Value = "+90 555 123 4567",
            ContactId = contactId
        };
        _contactInfoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ContactInformation>()))
            .ReturnsAsync(contactInfo);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(infoId, result.Value);
    }

    [Fact]
    public async Task DeleteContactInformationHandler_ExistingInformation_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteContactInformationHandler(_contactInfoRepositoryMock.Object);
        var infoId = Guid.NewGuid();
        var command = new DeleteContactInformationCommand(infoId);

        var contactInfo = new ContactInformation { Id = infoId, Type = ContactInfoType.Phone, Value = "123456789" };
        
        _contactInfoRepositoryMock.Setup(x => x.GetByIdAsync(infoId))
            .ReturnsAsync(contactInfo);
        _contactInfoRepositoryMock.Setup(x => x.DeleteAsync(infoId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteContactInformationHandler_NonExistingInformation_ReturnsFailure()
    {
        // Arrange
        var handler = new DeleteContactInformationHandler(_contactInfoRepositoryMock.Object);
        var infoId = Guid.NewGuid();
        var command = new DeleteContactInformationCommand(infoId);

        _contactInfoRepositoryMock.Setup(x => x.GetByIdAsync(infoId))
            .ReturnsAsync((ContactInformation?)null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("İletişim bilgisi bulunamadı", result.Error);
    }
}