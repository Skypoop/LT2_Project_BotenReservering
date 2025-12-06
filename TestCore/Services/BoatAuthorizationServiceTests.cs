using Moq;
using NUnit.Framework;
using FluentAssertions;
using ProjectBotenReservering.Core.Services;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace TestCore.Services;

[TestFixture]
public class BoatAuthorizationServiceTests
{
    private Mock<IClientService> _mockClientService;
    private Mock<IWeatherService> _mockWeatherService;
    private Mock<IBoatRepository> _mockBoatRepository;
    private Mock<IWindConstraintRepository> _mockWindConstraintRepository;
    private BoatAuthorizationService _service;

    [SetUp]
    public void Setup()
    {
        _mockClientService = new Mock<IClientService>();
        _mockWeatherService = new Mock<IWeatherService>();
        _mockBoatRepository = new Mock<IBoatRepository>();
        _mockWindConstraintRepository = new Mock<IWindConstraintRepository>();

        _service = new BoatAuthorizationService(
            _mockClientService.Object,
            _mockWeatherService.Object,
            _mockBoatRepository.Object,
            _mockWindConstraintRepository.Object
        );
    }

    // Tests for IsAuthorized(BoatType, int, Client?)

    [Test]
    public void IsAuthorized_WithClient_ReturnsTrue_WhenScullLevelIsSufficient()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 2, 0, "Club", true, "hash", 1);
        // Scull level 2, Boat level 2 -> Authorized

        // Act
        var result = _service.IsAuthorized(BoatType.S, 2, client);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_WithClient_ReturnsFalse_WhenScullLevelIsInsufficient()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 1, 0, "Club", true, "hash", 1);
        // Scull level 1, Boat level 2 -> Not Authorized

        // Act
        var result = _service.IsAuthorized(BoatType.S, 2, client);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_WithClient_ReturnsTrue_WhenSweepLevelIsSufficient()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 0, 3, "Club", true, "hash", 1);
        // Sweep level 3, Boat level 3 -> Authorized

        // Act
        var result = _service.IsAuthorized(BoatType.B, 3, client);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsAuthorized_WithClient_ReturnsFalse_WhenSweepLevelIsInsufficient()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 0, 2, "Club", true, "hash", 1);
        // Sweep level 2, Boat level 3 -> Not Authorized

        // Act
        var result = _service.IsAuthorized(BoatType.B, 3, client);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_WithClient_ReturnsFalse_WhenClientIsNull()
    {
        // Act
        var result = _service.IsAuthorized(BoatType.S, 1, null);

        // Assert
        result.Should().BeFalse();
    }

    // Tests for IsAuthorized(BoatType, int) - using IClientService

    [Test]
    public void IsAuthorized_UsesCurrentClient_ReturnsTrue_WhenAuthorized()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 2, 0, "Club", true, "hash", 1);
        _mockClientService.Setup(s => s.GetCurrentClient()).Returns(client);

        // Act
        var result = _service.IsAuthorized(BoatType.S, 2);

        // Assert
        result.Should().BeTrue();
        _mockClientService.Verify(s => s.GetCurrentClient(), Times.Once);
    }

    [Test]
    public void IsAuthorized_UsesCurrentClient_ReturnsFalse_WhenNotAuthorized()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 1, 0, "Club", true, "hash", 1);
        _mockClientService.Setup(s => s.GetCurrentClient()).Returns(client);

        // Act
        var result = _service.IsAuthorized(BoatType.S, 2);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsAuthorized_UsesCurrentClient_ReturnsFalse_WhenClientIsNull()
    {
        // Arrange
        _mockClientService.Setup(s => s.GetCurrentClient()).Returns((Client?)null);

        // Act
        var result = _service.IsAuthorized(BoatType.S, 1);

        // Assert
        result.Should().BeFalse();
    }

    // Tests for FilterAuthorized

    [Test]
    public void FilterAuthorized_ReturnsOnlyAuthorizedBoats()
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", 2, 1, "Club", true, "hash", 1);
        _mockClientService.Setup(s => s.GetCurrentClient()).Returns(client);

        var boats = new List<Boat>
        {
            new Boat("Boat1", true, 1, 2, BoatType.S, 80, true, "Club", 1), // Auth (Scull 2 >= 2)
            new Boat("Boat2", true, 1, 3, BoatType.S, 80, true, "Club", 2), // Not Auth (Scull 2 < 3)
            new Boat("Boat3", true, 1, 1, BoatType.B, 80, true, "Club", 3), // Auth (Sweep 1 >= 1)
            new Boat("Boat4", true, 1, 2, BoatType.B, 80, true, "Club", 4)  // Not Auth (Sweep 1 < 2)
        };

        // Act
        var result = _service.FilterAuthorized(boats, b => b.Type, b => b.Level).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "Boat1");
        result.Should().Contain(b => b.Name == "Boat3");
    }
}
