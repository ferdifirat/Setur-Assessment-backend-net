using Moq;
using ReportService.Application;
using ReportService.Domain;
using ReportService.Domain.Repositories;

namespace ReportService.Tests;

public class HandlerTests
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;

    public HandlerTests()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
    }

    [Fact]
    public async Task CreateReportHandler_ValidReport_ReturnsSuccess()
    {
        // Arrange
        var handler = new CreateReportHandler(_reportRepositoryMock.Object);
        var expectedReportId = Guid.NewGuid();
        var command = new CreateReportCommand(
            new CreateReportDto
            {
                ReportId = expectedReportId,
                RequestedAt = DateTime.UtcNow
            },
            "test-user"
        );

        var report = new Report
        {
            Id = expectedReportId,
            Status = ReportStatus.Preparing,
            RequestedAt = DateTime.UtcNow
        };
        _reportRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Report>()))
            .ReturnsAsync(report);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedReportId, result.Value);
        _reportRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Report>()), Times.Once);
    }

    [Fact]
    public async Task CreateReportHandler_InvalidReport_ReturnsFailure()
    {
        // Arrange
        var handler = new CreateReportHandler(_reportRepositoryMock.Object);
        var command = new CreateReportCommand(
            new CreateReportDto
            {
                ReportId = Guid.Empty,
                RequestedAt = DateTime.UtcNow
            },
            "test-user"
        );

        _reportRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Report>()))
            .ThrowsAsync(new InvalidOperationException("Validation failed"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Error);
    }

    [Fact]
    public async Task GetReportByIdHandler_ExistingReport_ReturnsSuccess()
    {
        // Arrange
        var handler = new GetReportByIdHandler(_reportRepositoryMock.Object);
        var reportId = Guid.NewGuid();
        var query = new GetReportByIdQuery(reportId);

        var report = new Report
        {
            Id = reportId,
            Status = ReportStatus.Completed,
            RequestedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow.AddHours(1)
        };

        _reportRepositoryMock.Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(report);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(reportId, result.Value.Id);
        Assert.Equal(ReportStatus.Completed, result.Value.Status);
    }

    [Fact]
    public async Task GetReportByIdHandler_NonExistingReport_ReturnsFailure()
    {
        // Arrange
        var handler = new GetReportByIdHandler(_reportRepositoryMock.Object);
        var reportId = Guid.NewGuid();
        var query = new GetReportByIdQuery(reportId);

        _reportRepositoryMock.Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Rapor bulunamadı", result.Error);
    }

    [Fact]
    public async Task GetAllReportsHandler_ReturnsSuccess()
    {
        // Arrange
        var handler = new GetAllReportsHandler(_reportRepositoryMock.Object);
        var query = new GetAllReportsQuery();

        var reports = new List<Report>
        {
            new() { Id = Guid.NewGuid(), Status = ReportStatus.Completed, RequestedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Status = ReportStatus.Preparing, RequestedAt = DateTime.UtcNow }
        };

        _reportRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(reports);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task UpdateReportStatusHandler_ValidUpdate_ReturnsSuccess()
    {
        // Arrange
        var handler = new UpdateReportStatusHandler(_reportRepositoryMock.Object);
        var reportId = Guid.NewGuid();
        var command = new UpdateReportStatusCommand(
            reportId,
            ReportStatus.Completed,
            "Sample statistics",
            "test-user"
        );

        var existingReport = new Report
        {
            Id = reportId,
            Status = ReportStatus.Preparing,
            RequestedAt = DateTime.UtcNow
        };

        _reportRepositoryMock.Setup(x => x.GetByIdAsync(reportId))
            .ReturnsAsync(existingReport);
        _reportRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Report>()))
            .ReturnsAsync(It.IsAny<Report>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _reportRepositoryMock.Verify(x => x.GetByIdAsync(reportId), Times.Once);
        _reportRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Report>()), Times.Once);
    }

    [Fact]
    public async Task UpdateReportStatusHandler_NonExistingReport_ReturnsFailure()
    {
        // Arrange
        var handler = new UpdateReportStatusHandler(_reportRepositoryMock.Object);
        var reportId = Guid.NewGuid();
        var command = new UpdateReportStatusCommand(
            reportId,
            ReportStatus.Completed,
            "Sample statistics",
            "test-user"
        );

        _reportRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Report>()))
            .ReturnsAsync(It.IsAny<Report>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Rapor bulunamadı", result.Error);
    }
}