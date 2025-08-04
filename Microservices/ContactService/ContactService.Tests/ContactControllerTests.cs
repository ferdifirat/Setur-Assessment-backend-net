using ContactService.Api.Properties;
using ContactService.Application;
using ContactService.Commands;
using ContactService.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Infrastructure.Messaging;
using Shared.Kernel.Events;
using Shared.Kernel.Results;

namespace ContactService.Tests;

public class ContactControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly ContactController _controller;

    public ContactControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _controller = new ContactController(_mediatorMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task Create_ValidContact_ReturnsCreatedResult()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = TestConstants.TestData.ValidFirstName,
            LastName = TestConstants.TestData.ValidLastName,
            Company = TestConstants.TestData.ValidCompany
        };

        var contactId = Guid.NewGuid();
        var result = Result<Guid>.Success(contactId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateContactCommand>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(contactId, createdResult.Value);
        Assert.Equal(nameof(ContactController.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Create_InvalidContact_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            FirstName = TestConstants.TestData.InvalidFirstName,
            LastName = TestConstants.TestData.ValidLastName
        };

        var result = Result<Guid>.Fail(TestConstants.ErrorMessages.FirstNameRequired);

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateContactCommand>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var errorResponse = Assert.IsType<Anonymous>(badRequestResult.Value);
        Assert.Equal(TestConstants.ErrorMessages.FirstNameRequired, errorResponse.Error);
    }

    [Fact]
    public async Task GetById_ExistingContact_ReturnsOkResult()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contactDto = new ContactDto
        {
            Id = contactId,
            FirstName = TestConstants.TestData.ValidFirstName,
            LastName = TestConstants.TestData.ValidLastName,
            Company = TestConstants.TestData.ValidCompany
        };

        var result = Result<ContactDto>.Success(contactDto);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetContactByIdQuery>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.GetById(contactId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedContact = Assert.IsType<ContactDto>(okResult.Value);
        Assert.Equal(contactId, returnedContact.Id);
    }

    [Fact]
    public async Task GetById_NonExistingContact_ReturnsNotFound()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var result = Result<ContactDto>.Fail(TestConstants.ErrorMessages.ContactNotFound);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetContactByIdQuery>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.GetById(contactId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var errorResponse = Assert.IsType<Anonymous>(notFoundResult.Value);
        Assert.Equal(TestConstants.ErrorMessages.ContactNotFound, errorResponse.Error);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Arrange
        var contacts = new List<ContactDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = TestConstants.TestData.ValidFirstName, LastName = TestConstants.TestData.ValidLastName },
            new() { Id = Guid.NewGuid(), FirstName = "Ayşe", LastName = "Fatma" }
        };

        var result = Result<List<ContactDto>>.Success(contacts);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllContactsQuery>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedContacts = Assert.IsType<List<ContactDto>>(okResult.Value);
        Assert.Equal(2, returnedContacts.Count);
    }

    [Fact]
    public async Task AddContactInformation_ValidInformation_ReturnsCreatedResult()
    {
        // Arrange
        var dto = new CreateContactInformationDto
        {
            ContactId = Guid.NewGuid(),
            Type = ContactInfoType.Phone,
            Value = TestConstants.TestData.ValidPhone
        };

        var infoId = Guid.NewGuid();
        var result = Result<Guid>.Success(infoId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateContactInformationCommand>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.AddContactInformation(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(infoId, createdResult.Value);
    }

    [Fact]
    public async Task DeleteContactInformation_ExistingInformation_ReturnsNoContent()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        var result = Result.Success();

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteContactInformationCommand>(), default))
            .Returns(Task.FromResult(result));

        // Act
        var actionResult = await _controller.DeleteContactInformation(infoId);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public async Task RequestReport_ReturnsAcceptedResult()
    {
        // Arrange
        _eventPublisherMock.Setup(x => x.PublishReportRequestedAsync(It.IsAny<ReportRequestedEvent>()))
            .Returns(Task.CompletedTask);

        // Act
        var actionResult = await _controller.RequestReport();

        // Assert
        var acceptedResult = Assert.IsType<AcceptedResult>(actionResult);
        var response = Assert.IsType<Anonymous>(acceptedResult.Value);
        Assert.NotNull(response.ReportId);
        Assert.NotEqual(Guid.Empty, response.ReportId);
    }

    private class Anonymous
    {
        public string Error { get; set; } = string.Empty;
        public Guid ReportId { get; set; }
    }
}