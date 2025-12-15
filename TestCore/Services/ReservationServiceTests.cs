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
        private Mock<IBoatRepository> _boatRepoMock;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IReservationRepository>();
            _authMock = new Mock<IBoatAuthorizationService>();
            _clientMock = new Mock<IClientReservationRepository>();
            _boatRepoMock = new Mock<IBoatRepository>();

            _service = new ReservationService(_repoMock.Object, _authMock.Object, _clientMock.Object, _boatRepoMock.Object);
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
        public void IsReservationTimeBlocked_WhenMatchingBoatReservationsReachCapacity_ReturnsTrue()
        {
            DateTime startTime = DateTime.Today.AddHours(9);
            DateTime endTime = startTime.AddHours(1);
            List<Reservation> reservations = new()
            {
                new Reservation(DateTime.Today, startTime, endTime, 1, 10, true)
            };
            BoatTypeUiItem boatType = CreateBoatType("Skiff", 1);

            _boatRepoMock.Setup(repo => repo.GetAll()).Returns(new List<Boat>
            {
                new Boat("Skiff", false, 1, 1, BoatType.S, 45, true, null, 10)
            });

            bool result = _service.IsReservationTimeBlocked(reservations, startTime, endTime, boatType);

            result.Should().BeTrue();
        }

        [Test]
        public void IsReservationTimeBlocked_IgnoresReservationsForDifferentBoatNames()
        {
            DateTime startTime = DateTime.Today.AddHours(9);
            DateTime endTime = startTime.AddHours(1);
            List<Reservation> reservations = new()
            {
                new Reservation(DateTime.Today, startTime, endTime, 1, 20, true)
            };
            BoatTypeUiItem boatType = CreateBoatType("Skiff", 1);

            _boatRepoMock.Setup(repo => repo.GetAll()).Returns(new List<Boat>
            {
                new Boat("Skiff", false, 1, 1, BoatType.S, 45, true, null, 10),
                new Boat("Other", false, 1, 1, BoatType.S, 45, true, null, 20)
            });

            bool result = _service.IsReservationTimeBlocked(reservations, startTime, endTime, boatType);

            result.Should().BeFalse();
        }

        private static BoatTypeUiItem CreateBoatType(string name, int amount)
        {
            return new BoatTypeUiItem
            {
                Id = 1,
                Name = name,
                SeatAmount = 1,
                Weight = 1,
                SteeringSeatPresent = false,
                Amount = amount,
                Level = 1,
                Type = BoatType.S
            };
        }
    }
}
