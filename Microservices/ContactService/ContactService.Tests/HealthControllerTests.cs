using ContactService.Api.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Xunit;

namespace ContactService.Tests;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        var healthCheckServiceMock = new Mock<HealthCheckService>();
        _controller = new HealthController(healthCheckServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult()
    {
        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<dynamic>(okResult.Value);
        Assert.Equal("Healthy", response.Status);
    }

    [Fact]
    public async Task Get_ReturnsCorrectStatusCode()
    {
        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
} 