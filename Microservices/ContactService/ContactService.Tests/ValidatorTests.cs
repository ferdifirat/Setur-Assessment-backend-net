using ContactService.Application;
using ContactService.Application.Validators;
using ContactService.Commands;
using ContactService.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace ContactService.Tests;

public class ValidatorTests
{
    [Fact]
    public void CreateContactCommandValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.FirstName);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.LastName);
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void CreateContactCommandValidator_EmptyFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "",
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.FirstName);
    }

    [Fact]
    public void CreateContactCommandValidator_NullFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = null,
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.FirstName);
    }

    [Fact]
    public void CreateContactCommandValidator_EmptyLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.LastName);
    }

    [Fact]
    public void CreateContactCommandValidator_NullLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "John",
                LastName = null,
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.LastName);
    }

    [Fact]
    public void CreateContactCommandValidator_NullCreatedBy_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            },
            null
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void CreateContactCommandValidator_LongFirstName_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = new string('A', 100),
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.FirstName);
    }

    [Fact]
    public void CreateContactCommandValidator_LongLastName_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactCommandValidator();
        var command = new CreateContactCommand(
            new CreateContactDto
            {
                FirstName = "John",
                LastName = new string('B', 100),
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.LastName);
    }

    [Fact]
    public void CreateContactDtoValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Company = "Test Company"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
        result.ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void CreateContactDtoValidator_EmptyFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "",
            LastName = "Doe",
            Company = "Test Company"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateContactDtoValidator_NullFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = null,
            LastName = "Doe",
            Company = "Test Company"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateContactDtoValidator_EmptyLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "",
            Company = "Test Company"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateContactDtoValidator_NullLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = null,
            Company = "Test Company"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateContactDtoValidator_NullCompany_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Company = null
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void CreateContactDtoValidator_EmptyCompany_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactDtoValidator();
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Company = ""
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.ContactId);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Type);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_EmptyContactId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.Empty,
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.ContactId);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_EmptyValue_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Phone,
                Value = ""
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_NullValue_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Phone,
                Value = null
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_InvalidContactInfoType_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = (ContactInfoType)999, // Invalid enum value
                Value = "+905551234567"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Type);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_ValidEmail_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Email,
                Value = "test@example.com"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_ValidPhone_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Phone,
                Value = "+905551234567"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationCommandValidator_ValidAddress_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationCommandValidator();
        var command = new CreateContactInformationCommand(
            new CreateContactInformationDto
            {
                ContactId = Guid.NewGuid(),
                Type = ContactInfoType.Address,
                Value = "Test Address, Istanbul, Turkey"
            }
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateContactInformationDtoValidator();
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = "+905551234567"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContactId);
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
        result.ShouldNotHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_EmptyContactId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationDtoValidator();
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.Empty,
            Type = ContactInfoType.Phone,
            Value = "+905551234567"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContactId);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_EmptyValue_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationDtoValidator();
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = ""
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void CreateContactInformationDtoValidator_NullValue_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new CreateContactInformationDtoValidator();
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = null
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void GetAllContactsQueryValidator_ValidQuery_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new GetAllContactsQueryValidator();
        var query = new GetAllContactsQuery();

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void GetContactByIdQueryValidator_ValidId_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new GetContactByIdQueryValidator();
        var query = new GetContactByIdQuery(Guid.NewGuid());

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void GetContactByIdQueryValidator_EmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new GetContactByIdQueryValidator();
        var query = new GetContactByIdQuery(Guid.Empty);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void UpdateContactCommandValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new UpdateContactCommandValidator();
        var command = new UpdateContactCommand(
            Guid.NewGuid(),
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.FirstName);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.LastName);
        result.ShouldNotHaveValidationErrorFor(x => x.UpdatedBy);
    }

    [Fact]
    public void UpdateContactCommandValidator_EmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new UpdateContactCommandValidator();
        var command = new UpdateContactCommand(
            Guid.Empty,
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void UpdateContactCommandValidator_EmptyFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new UpdateContactCommandValidator();
        var command = new UpdateContactCommand(
            Guid.NewGuid(),
            new CreateContactDto
            {
                FirstName = "",
                LastName = "Doe",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.FirstName);
    }

    [Fact]
    public void UpdateContactCommandValidator_EmptyLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new UpdateContactCommandValidator();
        var command = new UpdateContactCommand(
            Guid.NewGuid(),
            new CreateContactDto
            {
                FirstName = "John",
                LastName = "",
                Company = "Test Company"
            },
            "test-user"
        );

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.LastName);
    }

    [Fact]
    public void DeleteContactCommandValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new DeleteContactCommandValidator();
        var command = new DeleteContactCommand(Guid.NewGuid(), "test-user");

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.DeletedBy);
    }

    [Fact]
    public void DeleteContactCommandValidator_EmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new DeleteContactCommandValidator();
        var command = new DeleteContactCommand(Guid.Empty, "test-user");

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void DeleteContactInformationCommandValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new DeleteContactInformationCommandValidator();
        var command = new DeleteContactInformationCommand(Guid.NewGuid());

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void DeleteContactInformationCommandValidator_EmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new DeleteContactInformationCommandValidator();
        var command = new DeleteContactInformationCommand(Guid.Empty);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void GetContactInformationByContactIdQueryValidator_ValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new GetContactInformationByContactIdQueryValidator();
        var query = new GetContactInformationByContactIdQuery(Guid.NewGuid());

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContactId);
    }

    [Fact]
    public void GetContactInformationByContactIdQueryValidator_EmptyContactId_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new GetContactInformationByContactIdQueryValidator();
        var query = new GetContactInformationByContactIdQuery(Guid.Empty);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContactId);
    }
}