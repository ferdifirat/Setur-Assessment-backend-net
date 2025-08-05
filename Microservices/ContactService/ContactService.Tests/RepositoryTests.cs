using ContactService.Domain;
using ContactService.Infrastructure.Data;
using ContactService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ContactService.Tests;

public class RepositoryTests : IDisposable
{
    private readonly ContactDbContext _context;
    private readonly ContactRepository _contactRepository;
    private readonly ContactInformationRepository _contactInformationRepository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ContactDbContext(options);
        
        _contactRepository = new ContactRepository(_context);
        _contactInformationRepository = new ContactInformationRepository(_context);
    }

    [Fact]
    public async Task ContactRepository_AddAsync_ShouldAddContact()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "John",
            LastName = "Doe",
            Company = "Test Company"
        };

        // Act
        var result = await _contactRepository.AddAsync(contact);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task ContactRepository_GetByIdAsync_ShouldReturnContact()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "Jane",
            LastName = "Smith",
            Company = "Test Company"
        };
        await _contactRepository.AddAsync(contact);

        // Act
        var result = await _contactRepository.GetByIdAsync(contact.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contact.Id, result!.Id);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task ContactRepository_GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _contactRepository.GetByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ContactRepository_GetAllAsync_ShouldReturnAllContacts()
    {
        // Arrange
        var contact1 = new Contact { FirstName = "John", LastName = "Doe" };
        var contact2 = new Contact { FirstName = "Jane", LastName = "Smith" };
        await _contactRepository.AddAsync(contact1);
        await _contactRepository.AddAsync(contact2);

        // Act
        var result = await _contactRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ContactRepository_UpdateAsync_ShouldUpdateContact()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "Original",
            LastName = "Name"
        };
        await _contactRepository.AddAsync(contact);

        // Act
        contact.FirstName = "Updated";
        var result = await _contactRepository.UpdateAsync(contact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.FirstName);
    }

    [Fact]
    public async Task ContactRepository_DeleteAsync_ShouldDeleteContact()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "ToDelete",
            LastName = "Contact"
        };
        await _contactRepository.AddAsync(contact);

        // Act
        await _contactRepository.DeleteAsync(contact.Id);

        // Assert
        var deletedContact = await _contactRepository.GetByIdAsync(contact.Id);
        Assert.Null(deletedContact);
    }

    [Fact]
    public async Task ContactInformationRepository_AddAsync_ShouldAddContactInformation()
    {
        // Arrange
        var contact = new Contact { FirstName = "John", LastName = "Doe" };
        await _contactRepository.AddAsync(contact);

        var contactInfo = new ContactInformation
        {
            ContactId = contact.Id,
            Type = ContactInfoType.Phone,
            Value = "+905551234567"
        };

        // Act
        var result = await _contactInformationRepository.AddAsync(contactInfo);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(contact.Id, result.ContactId);
        Assert.Equal(ContactInfoType.Phone, result.Type);
    }

    [Fact]
    public async Task ContactInformationRepository_GetByContactIdAsync_ShouldReturnContactInformations()
    {
        // Arrange
        var contact = new Contact { FirstName = "John", LastName = "Doe" };
        await _contactRepository.AddAsync(contact);

        var contactInfo1 = new ContactInformation
        {
            ContactId = contact.Id,
            Type = ContactInfoType.Phone,
            Value = "+905551234567"
        };
        var contactInfo2 = new ContactInformation
        {
            ContactId = contact.Id,
            Type = ContactInfoType.Email,
            Value = "test@example.com"
        };
        await _contactInformationRepository.AddAsync(contactInfo1);
        await _contactInformationRepository.AddAsync(contactInfo2);

        // Act
        var result = await _contactInformationRepository.GetByContactIdAsync(contact.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, ci => ci.Type == ContactInfoType.Phone);
        Assert.Contains(result, ci => ci.Type == ContactInfoType.Email);
    }

    [Fact]
    public async Task ContactInformationRepository_GetByContactIdAsync_NonExistingContactId_ShouldReturnEmpty()
    {
        // Arrange
        var nonExistingContactId = Guid.NewGuid();

        // Act
        var result = await _contactInformationRepository.GetByContactIdAsync(nonExistingContactId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 