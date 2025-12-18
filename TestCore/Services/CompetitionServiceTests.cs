using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services
{
    [TestFixture]
    public class CompetitionServiceTests
    {
        private CompetitionService _competitionService;

        private Mock<IReservationService> _reservationServiceMock;
        private Mock<IClientService> _clientServiceMock;
        private Mock<IBoatRepository> _boatRepositoryMock;
        private Mock<ICompetitionRepository> _competitionRepositoryMock;
        private Mock<IReservationCompetitionRepository> _reservationCompetitionRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _reservationServiceMock = new Mock<IReservationService>();
            _clientServiceMock = new Mock<IClientService>();
            _boatRepositoryMock = new Mock<IBoatRepository>();
            _competitionRepositoryMock = new Mock<ICompetitionRepository>();
            _reservationCompetitionRepositoryMock = new Mock<IReservationCompetitionRepository>();

            _competitionService = new CompetitionService(
                _reservationServiceMock.Object,
                _clientServiceMock.Object,
                _boatRepositoryMock.Object,
                _competitionRepositoryMock.Object,
                _reservationCompetitionRepositoryMock.Object
            );
        }

        [Test]
        public void CreateCompetition_WithValidData_ReturnsCompetitionAndCreatesReservation()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddHours(1);
            DateTime endDate = startDate.AddHours(2);
            string competitionName = "Test competition";

            Client currentClient = new Client("John Doe", "john.doe@example.com", 2, 2, "Test Club", true, "hashedpassword", 1);

            Boat boat = new Boat("Test boat", true, 1, 1, BoatType.S, 1, true, "local", 1);

            Competition savedCompetition = new Competition(startDate, endDate, competitionName);
            
            savedCompetition.Id = 1;

            Reservation savedReservation = new Reservation(DateTime.Now, startDate, endDate, currentClient.Id, boat.Id, true
            );

            savedReservation.Id = 1;

            _clientServiceMock.Setup(x => x.GetCurrentClient()).Returns(currentClient);
            _boatRepositoryMock.Setup(x => x.Get(boat.Id)).Returns(boat);
            _competitionRepositoryMock.Setup(x => x.Add(It.IsAny<Competition>())).Returns(savedCompetition);
            _reservationServiceMock.Setup(x => x.CreateReservation(It.IsAny<Reservation>(), It.IsAny<List<Client>>())).Returns(savedReservation);
            _reservationCompetitionRepositoryMock.Setup(x => x.Add(It.IsAny<ReservationCompetition>())).Returns((ReservationCompetition rc) => rc);
            _competitionService.ClearCompetitionBoats();
            _competitionService.AmountBoats = 1;
            _competitionService.SelectedBoatId = boat.Id;

            // Act
            Competition? result = _competitionService.CreateCompetition(startDate, endDate, competitionName);

            // Assert
            result.Should().NotBeNull();
            result!.CompetitionName.Should().Be(competitionName);
            result.StartDateTime.Should().Be(startDate);
            result.EndDateTime.Should().Be(endDate);

            _reservationServiceMock.Verify(x => x.CreateReservation(It.IsAny<Reservation>(), It.IsAny<List<Client>>()), Times.Once);
            _competitionRepositoryMock.Verify(x => x.Add(It.IsAny<Competition>()), Times.Once);
            _reservationCompetitionRepositoryMock.Verify(x => x.Add(It.IsAny<ReservationCompetition>()), Times.Once);
        }
    }
}