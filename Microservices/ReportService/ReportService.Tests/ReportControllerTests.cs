using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportService.Api.Controllers;
using ReportService.Application;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Tests;

public class ReportControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ReportController _controller;

    public ReportControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ReportController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Arrange
        var reports = new List<ReportDto>
        {
            new() { Id = Guid.NewGuid(), Status = ReportStatus.Completed, RequestedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Status = ReportStatus.Preparing, RequestedAt = DateTime.UtcNow }
        };

        var result = Result<List<ReportDto>>.Success(reports);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllReportsQuery>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedReports = Assert.IsType<List<ReportDto>>(okResult.Value);
        Assert.Equal(2, returnedReports.Count);
    }

    [Fact]
    public async Task GetAll_Error_ReturnsBadRequest()
    {
        // Arrange
        var result = Result<List<ReportDto>>.Fail(TestConstants.ErrorMessages.ReportNotFound);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllReportsQuery>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetAll();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        
        // Use reflection to check the anonymous type properties
        var errorProperty = badRequestResult.Value.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        
        var error = (string)errorProperty.GetValue(badRequestResult.Value);
        Assert.Equal(TestConstants.ErrorMessages.ReportNotFound, error);
    }

    [Fact]
    public async Task GetById_ExistingReport_ReturnsOkResult()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var reportDto = new ReportDto
        {
            Id = reportId,
            Status = ReportStatus.Completed,
            RequestedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow.AddHours(1)
        };

        var result = Result<ReportDto>.Success(reportDto);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetReportByIdQuery>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetById(reportId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedReport = Assert.IsType<ReportDto>(okResult.Value);
        Assert.Equal(reportId, returnedReport.Id);
        Assert.Equal(ReportStatus.Completed, returnedReport.Status);
    }

    [Fact]
    public async Task GetById_NonExistingReport_ReturnsNotFound()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var result = Result<ReportDto>.Fail(TestConstants.ErrorMessages.ReportNotFound);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetReportByIdQuery>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetById(reportId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        
        // Use reflection to check the anonymous type properties
        var errorProperty = notFoundResult.Value.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        
        var error = (string)errorProperty.GetValue(notFoundResult.Value);
        Assert.Equal("Rapor bulunamadı.", error);
    }

    [Fact]
    public async Task Create_ValidReport_ReturnsCreatedResult()
    {
        // Arrange
        var dto = new CreateReportDto
        {
            ReportId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow
        };

        var reportId = Guid.NewGuid();
        var result = Result<Guid>.Success(reportId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateReportCommand>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(reportId, createdResult.Value);
        Assert.Equal(nameof(ReportController.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Create_InvalidReport_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateReportDto
        {
            ReportId = Guid.Empty,
            RequestedAt = DateTime.UtcNow
        };

        var result = Result<Guid>.Fail("Geçersiz rapor ID.");

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateReportCommand>(), default))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Create(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        
        // Use reflection to get the Error property value from the anonymous type
        var errorProperty = badRequestResult.Value.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        
        var errorValue = errorProperty.GetValue(badRequestResult.Value) as string;
        Assert.Equal("Geçersiz rapor ID.", errorValue);
    }


}