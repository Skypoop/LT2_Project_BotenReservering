using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Constants;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services
{
    public class ReservationServiceTests
    {
        private ReservationService _service;
        private Mock<IReservationRepository> _repoMock;
        private Mock<IBoatAuthorizationService> _authMock;
        private Mock<IClientReservationRepository> _clientMock;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IReservationRepository>();
            _authMock = new Mock<IBoatAuthorizationService>();
            _clientMock = new Mock<IClientReservationRepository>();

            _service = new ReservationService(_repoMock.Object, _authMock.Object, _clientMock.Object);
        }

        [Test]
        public void IsBookingWithinAllowedReservationTime_WhenBookingDateIsWithinMaxDays_ReturnsTrue()
        {
            //Arrange
            int allowedDays = ReservationRules.MaxDaysBeforeReservation;
            DateTime startTime = DateTime.Today.AddDays(allowedDays - 1);

            //Act
            bool result = _service.IsBookingWithinAllowedReservationTime(startTime);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsBookingWithinAllowedReservationTime_WhenBookingDateIsBeyondMaxDays_ReturnsFalse()
        {
            //Arrange
            int allowedDays = ReservationRules.MaxDaysBeforeReservation;
            DateTime startTime = DateTime.Today.AddDays(allowedDays + 1);

            //Act
            bool result = _service.IsBookingWithinAllowedReservationTime(startTime);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValidReservationLength_WhenDurationIsWithinLimit_ReturnsTrue()
        {
            //Arrange
            DateTime startTime = DateTime.Today.AddDays(1);
            DateTime endTime = DateTime.Today.AddDays(1).AddMinutes(110);

            //Act
            bool result = _service.IsValidReservationLength(startTime, endTime);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsValidReservationLength_WhenDurationExceedsLimit_ReturnsFalse()
        {
            //Arrange
            DateTime startTime = DateTime.Today.AddDays(1);
            DateTime endTime = DateTime.Today.AddDays(1).AddMinutes(130);

            //Act
            bool result = _service.IsValidReservationLength(startTime, endTime);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void CreateReservation_SetsApprovedToFalse_WhenClientIsNotAuthorized()
        {
            // Arrange
            Reservation reservation = new Reservation(DateTime.Now, DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 1, 1, true);
            List<Client> clients = new List<Client> { new Client("Test", "test@test.com", 1, 1, "Club", true, "hash", 1) };

            _authMock.Setup(a => a.IsAuthorized(It.IsAny<int>(), It.IsAny<Client>())).Returns(false);

            // Act
            _service.CreateReservation(reservation, clients);

            // Assert
            reservation.Approved.Should().BeFalse();
            _repoMock.Verify(r => r.Add(reservation), Times.Once);
            _clientMock.Verify(c => c.Add(It.IsAny<ClientReservation>()), Times.Once);
        }

        [Test]
        public void CreateReservation_KeepsApprovedTrue_WhenAllClientsAreAuthorized()
        {
            // Arrange
            Reservation reservation = new Reservation(DateTime.Now, DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), 1, 1, true);
            List<Client> clients = new List<Client> { new Client("Test", "test@test.com", 1, 1, "Club", true, "hash", 1) };

            _authMock.Setup(a => a.IsAuthorized(It.IsAny<int>(), It.IsAny<Client>())).Returns(true);

            // Act
            _service.CreateReservation(reservation, clients);

            // Assert
            reservation.Approved.Should().BeTrue();
            _repoMock.Verify(r => r.Add(reservation), Times.Once);
            _clientMock.Verify(c => c.Add(It.IsAny<ClientReservation>()), Times.Once);
        }

        [Test]
        public void FindOverlappingReservationForCompetition_WithOverlappingReservations_ReturnsOverlappingReservations()
        {
            // Arrange
            DateTime startDateCompetition = new DateTime(2025, 12, 12, 10, 0, 0);
            DateTime endDateCompetition = new DateTime(2025, 12, 12, 12, 0, 0);
            List<int> boatIds = new List<int> { 1, 2, 3 };

            List<Reservation> allReservations = new List<Reservation>
        {
            // Overlaps: starts before competition, ends during competition
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 8, 0, 0), new DateTime(2025, 12, 12, 10, 0, 0), 1, 1, true, 4),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), new DateTime(2025, 12, 12, 14, 0, 0), 1, 2, true, 5)
        };

            _repoMock.Setup(r => r.GetAll()).Returns(allReservations);

            // Act
            List<Reservation> result = _service.FindOverlappingReservations(startDateCompetition, endDateCompetition, boatIds);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(r => r.Id == 1);
            result.Should().Contain(r => r.Id == 2);
            result.Should().Contain(r => r.Id == 3);
            result.Should().NotContain(r => r.Id == 4);
            result.Should().NotContain(r => r.Id == 5);
        }

        [Test]
        public void FindOverlappingReservationForCompetition_WithNoOverlappingReservations_ReturnsEmptyList()
        {
            // Arrange
            DateTime startDateCompetition = new DateTime(2025, 12, 12, 10, 0, 0);
            DateTime endDateCompetition = new DateTime(2025, 12, 12, 12, 0, 0);
            List<int> boatIds = new List<int> { 1, 2, 3 };

            List<Reservation> allReservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 8, 0, 0), new DateTime(2025, 12, 12, 10, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), new DateTime(2025, 12, 12, 14, 0, 0), 1, 2, true, 2)
        };

            _repoMock.Setup(r => r.GetAll()).Returns(allReservations);

            // Act
            List<Reservation> result = _service.FindOverlappingReservations(startDateCompetition, endDateCompetition, boatIds);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void FindOverlappingReservationForCompetition_WithDifferentBoatIds_ReturnsOnlyCompetitioningBoats()
        {
            // Arrange
            DateTime startDateCompetition = new DateTime(2025, 12, 12, 10, 0, 0);
            DateTime endDateCompetition = new DateTime(2025, 12, 12, 12, 0, 0);
            List<int> boatIds = new List<int> { 1, 2 }; // Only boats 1 and 2

            List<Reservation> allReservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            // Boat 3 is NOT in boatIds list
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3)
        };

            _repoMock.Setup(r => r.GetAll()).Returns(allReservations);

            // Act
            List<Reservation> result = _service.FindOverlappingReservations(startDateCompetition, endDateCompetition, boatIds);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.BoatId == 1);
            result.Should().Contain(r => r.BoatId == 2);
            result.Should().NotContain(r => r.BoatId == 3);
        }

        [Test]
        public void FindOverlappingReservationForCompetition_WithEmptyReservationsList_ReturnsEmptyList()
        {
            // Arrange
            DateTime startDateCompetition = new DateTime(2025, 12, 12, 10, 0, 0);
            DateTime endDateCompetition = new DateTime(2025, 12, 12, 12, 0, 0);
            List<int> boatIds = new List<int> { 1, 2, 3 };

            _repoMock.Setup(r => r.GetAll()).Returns(new List<Reservation>());

            // Act
            List<Reservation> result = _service.FindOverlappingReservations(startDateCompetition, endDateCompetition, boatIds);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void DeleteOverlappingReservationForCompetition_WithMultipleReservations_CallsCancelForEachReservation()
        {
            List<Reservation> reservations = new List<Reservation>
        {
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), 1, 1, true, 1),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 11, 0, 0), new DateTime(2025, 12, 12, 13, 0, 0), 1, 2, true, 2),
            new Reservation(new DateTime(2025, 12, 10, 0, 0, 0), new DateTime(2025, 12, 12, 9, 0, 0), new DateTime(2025, 12, 12, 12, 0, 0), 1, 3, true, 3)
        };

            // Act
            _service.CancelOverlappingReservations(reservations);

            // Assert
            _repoMock.Verify(m => m.CancelReservationsByIds(It.Is<List<int>>(list => list.Count == 3 && list.Contains(1) && list.Contains(2) && list.Contains(3))), Times.Once);
        }

        [Test]
        public void DeleteOverlappingReservationForCompetition_WithEmptyList_DoesNotCallCancel()
        {
            // Arrange
            List<Reservation> reservations = new List<Reservation>();

            // Act
            _service.CancelOverlappingReservations(reservations);
            // Assert
            _repoMock.Verify(m => m.CancelReservationsByIds(It.IsAny<List<int>>()), Times.Once);
        }
    }
}
