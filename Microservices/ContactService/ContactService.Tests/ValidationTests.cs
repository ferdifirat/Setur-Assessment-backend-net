using ContactService.Application;
using ContactService.Application.Validators;
using ContactService.Domain;
using FluentValidation.TestHelper;

namespace ContactService.Tests;

public class ValidationTests
{
    private readonly CreateContactDtoValidator _contactValidator;
    private readonly CreateContactInformationDtoValidator _contactInfoValidator;

    public ValidationTests()
    {
        _contactValidator = new CreateContactDtoValidator();
        _contactInfoValidator = new CreateContactInformationDtoValidator();
    }

    [Fact]
    public void CreateContactDtoValidator_ValidContact_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = "Ali",
            LastName = "Veli",
            Company = "ABC Şirketi"
        };

        // Act
        var result = _contactValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateContactDtoValidator_EmptyFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = "",
            LastName = "Veli"
        };

        // Act
        var result = _contactValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateContactDtoValidator_FirstNameWithNumbers_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = "Ali123",
            LastName = "Veli"
        };

        // Act
        var result = _contactValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateContactDtoValidator_FirstNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = new string('A', 101), // 101 karakter
            LastName = "Veli"
        };

        // Act
        var result = _contactValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_ValidPhone_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = "+90 555 123 4567"
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateContactInformationDtoValidator_InvalidPhone_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = "555 123 4567" // +90 eksik
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_ValidEmail_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Email,
            Value = "test@example.com"
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateContactInformationDtoValidator_InvalidEmail_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Email,
            Value = "invalid-email"
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_ValidLocation_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Location,
            Value = "İstanbul, Türkiye"
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateContactInformationDtoValidator_ShortLocation_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Location,
            Value = "AB" // 2 karakter - minimum 3 olmalı
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_EmptyValue_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = ""
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_EmptyContactId_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.Empty,
            Type = ContactInfoType.Phone,
            Value = "+90 555 123 4567"
        };

        // Act
        var result = _contactInfoValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContactId);
    }
}