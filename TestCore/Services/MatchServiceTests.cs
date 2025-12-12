using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services;

[TestFixture]
public class MatchServiceTests
{
    private Mock<IReservationRepository> _reservationRepository;
    private Mock<IMatchRepository> _matchRepository;
    private CompetitionService _matchService;

    [SetUp]
    public void SetUp()
    {
        _reservationRepository = new Mock<IReservationRepository>();
        _matchRepository = new Mock<IMatchRepository>();
        _matchService = new MatchService(_reservationRepository.Object, _matchRepository.Object);
    }

    [Test]
    public void FindOverlappingReservationForMatch_WithOverlappingReservations_ReturnsOverlappingReservations()
    {
        // Arrange
        DateTime startDateMatch = new DateTime(2025, 12, 12, 10, 0, 0);
        DateTime endDateMatch = new DateTime(2025, 12, 12, 12, 0, 0);
        List<int> boatIds = new List<int> { 1, 2, 3 };

        List<Reservation> allReservations = new List<Reservation>
        {
            // Overlaps: starts before match, ends during match
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            // Overlaps: starts during match, ends after match
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            // Overlaps: starts before match, ends at match end
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3),
            // Does NOT overlap: ends before match starts
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 8, 0, 0), new DateTime(2025, 12, 12, 10, 0, 0), 1, 1, true, 4),
            // Does NOT overlap: starts after match ends
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), new DateTime(2025, 12, 12, 14, 0, 0), 1, 2, true, 5)
        };

        _reservationRepository.Setup(r => r.GetAll()).Returns(allReservations);

        // Act
        List<Reservation> result = _matchService.FindOverlappingReservationsForMatch(startDateMatch, endDateMatch, boatIds);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(r => r.Id == 1);
        result.Should().Contain(r => r.Id == 2);
        result.Should().Contain(r => r.Id == 3);
        result.Should().NotContain(r => r.Id == 4);
        result.Should().NotContain(r => r.Id == 5);
    }

    [Test]
    public void FindOverlappingReservationForMatch_WithNoOverlappingReservations_ReturnsEmptyList()
    {
        // Arrange
        DateTime startDateMatch = new DateTime(2025, 12, 12, 10, 0, 0);
        DateTime endDateMatch = new DateTime(2025, 12, 12, 12, 0, 0);
        List<int> boatIds = new List<int> { 1, 2, 3 };

        List<Reservation> allReservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 8, 0, 0), new DateTime(2025, 12, 12, 10, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), new DateTime(2025, 12, 12, 14, 0, 0), 1, 2, true, 2)
        };

        _reservationRepository.Setup(r => r.GetAll()).Returns(allReservations);

        // Act
        List<Reservation> result = _matchService.FindOverlappingReservationsForMatch(startDateMatch, endDateMatch, boatIds);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void FindOverlappingReservationForMatch_WithDifferentBoatIds_ReturnsOnlyMatchingBoats()
    {
        // Arrange
        DateTime startDateMatch = new DateTime(2025, 12, 12, 10, 0, 0);
        DateTime endDateMatch = new DateTime(2025, 12, 12, 12, 0, 0);
        List<int> boatIds = new List<int> { 1, 2 }; // Only boats 1 and 2

        List<Reservation> allReservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            // Boat 3 is NOT in boatIds list
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3)
        };

        _reservationRepository.Setup(r => r.GetAll()).Returns(allReservations);

        // Act
        List<Reservation> result = _matchService.FindOverlappingReservationsForMatch(startDateMatch, endDateMatch, boatIds);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.BoatId == 1);
        result.Should().Contain(r => r.BoatId == 2);
        result.Should().NotContain(r => r.BoatId == 3);
    }

    [Test]
    public void FindOverlappingReservationForMatch_WithEmptyReservationsList_ReturnsEmptyList()
    {
        // Arrange
        DateTime startDateMatch = new DateTime(2025, 12, 12, 10, 0, 0);
        DateTime endDateMatch = new DateTime(2025, 12, 12, 12, 0, 0);
        List<int> boatIds = new List<int> { 1, 2, 3 };

        _reservationRepository.Setup(r => r.GetAll()).Returns(new List<Reservation>());

        // Act
        List<Reservation> result = _matchService.FindOverlappingReservationsForMatch(startDateMatch, endDateMatch, boatIds);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void DeleteOverlappingReservationForMatch_WithMultipleReservations_CallsCancelForEachReservation()
    {
        // Arrange
        int matchId = 100;
        List<Reservation> reservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3)
        };

        // Act
        _matchService.CancelOverlappingReservationsForMatch(reservations);

        // Assert
        _matchRepository.Verify(m => m.CancelReservationAndUpdateStatus(1), Times.Once);
        _matchRepository.Verify(m => m.CancelReservationAndUpdateStatus(2), Times.Once);
        _matchRepository.Verify(m => m.CancelReservationAndUpdateStatus(3), Times.Once);
        _matchRepository.Verify(m => m.CancelReservationAndUpdateStatus(It.IsAny<int>()), Times.Exactly(3));
    }

    [Test]
    public void DeleteOverlappingReservationForMatch_WithEmptyList_DoesNotCallCancel()
    {
        // Arrange
        int matchId = 100;
        List<Reservation> reservations = new List<Reservation>();

        // Act
        _matchService.CancelOverlappingReservationsForMatch(reservations);

        // Assert
        _matchRepository.Verify(m => m.CancelReservationAndUpdateStatus(It.IsAny<int>()), Times.Never);
    }
}