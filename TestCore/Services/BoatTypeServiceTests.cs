using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services;

[TestFixture]
public class BoatTypeServiceTests
{
    private Mock<IBoatRepository> _mockBoatRepository;
    private Mock<IBoatAuthorizationService> _mockBoatAuthorizationService;
    private BoatTypeService _service;

    [SetUp]
    public void Setup()
    {
        _mockBoatRepository = new Mock<IBoatRepository>();
        _mockBoatAuthorizationService = new Mock<IBoatAuthorizationService>();
        _service = new BoatTypeService(_mockBoatRepository.Object, _mockBoatAuthorizationService.Object);
    }

    [Test]
    public void GetBoatTypes_ReturnsOnlyAuthorizedBoats()
    {
        // Arrange
        List<Boat> allBoats = new List<Boat>
        {
            new Boat("Boat1", true, 1, 2, BoatType.S, 80, true, "Club", 1),
            new Boat("Boat2", true, 1, 3, BoatType.S, 80, true, "Club", 2)
        };

        List<Boat> authorizedBoats = new List<Boat>
        {
            allBoats[0]
        };

        _mockBoatRepository.Setup(r => r.GetAll()).Returns(allBoats);

        // Setup FilterAuthorized to return the authorized list
        _mockBoatAuthorizationService
            .Setup(s => s.FilterAuthorized(
                It.IsAny<IEnumerable<Boat>>(),
                It.IsAny<Func<Boat, BoatType>>(),
                It.IsAny<Func<Boat, int>>()))
            .Returns(authorizedBoats);

        // Act
        List<BoatTypeUiItem> result = _service.GetBoatTypes();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Boat1");

        _mockBoatRepository.Verify(r => r.GetAll(), Times.Once);
        _mockBoatAuthorizationService.Verify(s => s.FilterAuthorized(
            It.IsAny<IEnumerable<Boat>>(),
            It.IsAny<Func<Boat, BoatType>>(),
            It.IsAny<Func<Boat, int>>()), Times.Once);
    }
}
