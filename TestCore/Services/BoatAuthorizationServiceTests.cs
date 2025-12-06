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

    [TestCase(BoatType.S, 2, 2, 0, true, TestName = "IsAuthorized_WithClient_Scull_Sufficient_ReturnsTrue")]
    [TestCase(BoatType.S, 2, 1, 0, false, TestName = "IsAuthorized_WithClient_Scull_Insufficient_ReturnsFalse")]
    [TestCase(BoatType.B, 3, 0, 3, true, TestName = "IsAuthorized_WithClient_Sweep_Sufficient_ReturnsTrue")]
    [TestCase(BoatType.B, 3, 0, 2, false, TestName = "IsAuthorized_WithClient_Sweep_Insufficient_ReturnsFalse")]
    public void IsAuthorized_WithClient_ChecksLevelsCorrectly(BoatType boatType, int boatLevel, int scullLevel, int sweepLevel, bool expected)
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", scullLevel, sweepLevel, "Club", true, "hash", 1);

        // Act
        var result = _service.IsAuthorized(boatType, boatLevel, client);

        // Assert
        result.Should().Be(expected);
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

    [TestCase(BoatType.S, 2, 2, 0, true, TestName = "IsAuthorized_CurrentClient_Scull_Sufficient_ReturnsTrue")]
    [TestCase(BoatType.S, 2, 1, 0, false, TestName = "IsAuthorized_CurrentClient_Scull_Insufficient_ReturnsFalse")]
    public void IsAuthorized_UsesCurrentClient_ChecksLevelsCorrectly(BoatType boatType, int boatLevel, int scullLevel, int sweepLevel, bool expected)
    {
        // Arrange
        var client = new Client("Test User", "test@example.com", scullLevel, sweepLevel, "Club", true, "hash", 1);
        _mockClientService.Setup(s => s.GetCurrentClient()).Returns(client);

        // Act
        var result = _service.IsAuthorized(boatType, boatLevel);

        // Assert
        result.Should().Be(expected);
        _mockClientService.Verify(s => s.GetCurrentClient(), Times.Once);
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
    public void FilterAuthorized_WhenListContainsMixedAuthorization_ReturnsOnlyAuthorizedBoats()
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
