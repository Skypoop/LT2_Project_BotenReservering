using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Constants;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services
{
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
            // EERST de mocks initialiseren - dit was het probleem!
            _reservationServiceMock = new Mock<IReservationService>();
            _clientServiceMock = new Mock<IClientService>();
            _boatRepositoryMock = new Mock<IBoatRepository>();
            _competitionRepositoryMock = new Mock<ICompetitionRepository>();
            _reservationCompetitionRepositoryMock = new Mock<IReservationCompetitionRepository>();

            // DAN pas de service aanmaken
            _competitionService = new CompetitionService(
                _reservationServiceMock.Object,
                _clientServiceMock.Object,
                _boatRepositoryMock.Object,
                _competitionRepositoryMock.Object,
                _reservationCompetitionRepositoryMock.Object
            );
        }

        [Test]
        public void CreateMatch_Returns_MatchObject()
        {
            // Arrange
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddHours(2);
            string competitionName = "Test competition";

            Client? currentClient = new Client("John Doe", "john.doe@example.com", 2, 2, "Test Club", true, "hashedpassword", 1);
            Boat? boat = new Boat("Test Boot 1", true, 1, 1, BoatType.S, 1, true, "local", 1);
            Competition? expectedCompetition = new Competition(startDate, endDate, competitionName, 1);
            Reservation? expectedReservation = new Reservation(DateTime.Now, startDate, endDate, currentClient.Id, boat.Id, true, 1);

            _clientServiceMock.Setup(x => x.GetCurrentClient()).Returns(currentClient);
            _boatRepositoryMock.Setup(x => x.Get(It.IsAny<int>())).Returns(boat);
            _reservationServiceMock.Setup(x => x.CreateReservation(It.IsAny<Reservation>(), It.IsAny<List<Client>>())).Returns(expectedReservation);
            _competitionRepositoryMock.Setup(x => x.Add(It.IsAny<Competition>())).Returns(expectedCompetition);
            _reservationCompetitionRepositoryMock.Setup(x => x.Add(It.IsAny<ReservationCompetition>())).Returns((ReservationCompetition rc) => rc);
            _competitionService.AmountBoats = 1;
            _competitionService.SelectedBoatId = boat.Id;

            // Act
            Competition? result = _competitionService.CreateCompetition(startDate, endDate, competitionName);

            // Assert
            result.Should().NotBeNull();
            result!.StartDateTime.Should().Be(startDate);
            result.EndDateTime.Should().Be(endDate);
            result.CompetitionName.Should().Be(competitionName);
        }
    }
}