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
        private CompetitionService _service;
        private Mock<IReservationService> _reservationServiceMock;
        private Mock<IClientService> _clientServiceMock;
        private Mock<IBoatRepository> _boatRepoMock;
        private Mock<ICompetitionRepository> _competitionRepoMock;
        private Mock<IReservationCompetitionRepository> _reservationCompetitionRepoMock;

        [SetUp]
        public void Setup()
        {
            _reservationServiceMock = new Mock<IReservationService>();
            _clientServiceMock = new Mock<IClientService>();
            _boatRepoMock = new Mock<IBoatRepository>();
            _competitionRepoMock = new Mock<ICompetitionRepository>();
            _reservationCompetitionRepoMock = new Mock<IReservationCompetitionRepository>();

            _service = new CompetitionService(
                _reservationServiceMock.Object,
                _clientServiceMock.Object,
                _boatRepoMock.Object,
                _competitionRepoMock.Object,
                _reservationCompetitionRepoMock.Object
            );
        }


        [Test]
        public void ValidateCompetition_WhenEndDateIsAfterStartDate_ReturnsValid()
        {
            // Arrange
            DateTime start = DateTime.Now.AddDays(1);
            DateTime end = DateTime.Now.AddDays(1).AddHours(2);
            List<Boat> boats = new List<Boat> { CreateBoat("TestBoat", 1) };

            // Act
            ValueTuple<bool, string?> result = _service.ValidateCompetition(start, end, boats);

            // Assert
            result.Item1.Should().BeTrue();
            result.Item2.Should().BeNull();
        }

        [Test]
        public void ValidateCompetition_WhenEndDateIsBeforeStartDate_ReturnsInvalid()
        {
            // Arrange
            DateTime start = DateTime.Now.AddDays(1).AddHours(2);
            DateTime end = DateTime.Now.AddDays(1);
            List<Boat> boats = new List<Boat> { CreateBoat("TestBoat", 1) };

            // Act
            ValueTuple<bool, string?> result = _service.ValidateCompetition(start, end, boats);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Be("De einddatum moet later zijn dan de begindatum.");
        }

        [Test]
        public void ValidateCompetition_WhenStartDateIsInPast_ReturnsInvalid()
        {
            // Arrange
            DateTime start = DateTime.Now.AddDays(-1);
            DateTime end = DateTime.Now.AddDays(1);
            List<Boat> boats = new List<Boat> { CreateBoat("TestBoat", 1) };

            // Act
            ValueTuple<bool, string?> result = _service.ValidateCompetition(start, end, boats);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Be("De begindatum mag niet in het verleden liggen.");
        }

        [Test]
        public void ValidateCompetition_WhenNoBoatsSelected_ReturnsInvalid()
        {
            // Arrange
            DateTime start = DateTime.Now.AddDays(1);
            DateTime end = DateTime.Now.AddDays(1).AddHours(2);
            List<Boat> boats = new List<Boat>();

            // Act
            ValueTuple<bool, string?> result = _service.ValidateCompetition(start, end, boats);

            // Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Be("Er zijn geen boten geselecteerd.");
        }

        [Test]
        public void HasEnoughBoats_WhenEnoughBoatsAvailable_ReturnsTrue()
        {
            // Arrange
            int boatId = 1;
            _service.AmountBoats = 2;

            Boat referenceBoat = CreateBoat("Skiff", boatId);
            List<Boat> allSkiffs = new List<Boat>
            {
                CreateBoat("Skiff", 1),
                CreateBoat("Skiff", 2),
                CreateBoat("Skiff", 3)
            };

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns(referenceBoat);
            _boatRepoMock.Setup(r => r.GetAllFromName("Skiff")).Returns(allSkiffs);

            // Act
            bool result = _service.HasEnoughBoats(boatId);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void HasEnoughBoats_WhenNotEnoughBoatsAvailable_ReturnsFalse()
        {
            // Arrange
            int boatId = 1;
            _service.AmountBoats = 5;

            Boat referenceBoat = CreateBoat("Skiff", boatId);
            List<Boat> allSkiffs = new List<Boat>
            {
                CreateBoat("Skiff", 1),
                CreateBoat("Skiff", 2)
            };

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns(referenceBoat);
            _boatRepoMock.Setup(r => r.GetAllFromName("Skiff")).Returns(allSkiffs);

            // Act
            bool result = _service.HasEnoughBoats(boatId);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasEnoughBoats_WhenBoatDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int boatId = 999;
            _service.AmountBoats = 1;

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns((Boat?)null);

            // Act
            bool result = _service.HasEnoughBoats(boatId);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SetSelectedBoat_WhenBoatsHaveOverlappingReservations_ReturnsTrue()
        {
            // Arrange
            int boatId = 1;
            DateTime startTime = DateTime.Today.AddDays(1).AddHours(10);
            DateTime endTime = DateTime.Today.AddDays(1).AddHours(12);
            _service.AmountBoats = 2;

            Boat referenceBoat = CreateBoat("Skiff", boatId);
            List<Boat> allSkiffs = new List<Boat>
            {
                CreateBoat("Skiff", 1),
                CreateBoat("Skiff", 2)
            };

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns(referenceBoat);
            _boatRepoMock.Setup(r => r.GetAllFromName("Skiff")).Returns(allSkiffs);

            Dictionary<Boat, int> overlapCounts = new Dictionary<Boat, int>
            {
                { allSkiffs[0], 1 },
                { allSkiffs[1], 0 }
            };
            _reservationServiceMock.Setup(r => r.CountOverlappingActiveReservations(
                It.IsAny<List<Boat>>(), startTime, endTime)).Returns(overlapCounts);

            // Act
            bool hasOverlap = _service.SetSelectedBoat(boatId, startTime, endTime);

            // Assert
            hasOverlap.Should().BeTrue();
        }

        [Test]
        public void SetSelectedBoat_WhenNoOverlappingReservations_ReturnsFalse()
        {
            // Arrange
            int boatId = 1;
            DateTime startTime = DateTime.Today.AddDays(1).AddHours(10);
            DateTime endTime = DateTime.Today.AddDays(1).AddHours(12);
            _service.AmountBoats = 2;

            Boat referenceBoat = CreateBoat("Skiff", boatId);
            List<Boat> allSkiffs = new List<Boat>
            {
                CreateBoat("Skiff", 1),
                CreateBoat("Skiff", 2)
            };

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns(referenceBoat);
            _boatRepoMock.Setup(r => r.GetAllFromName("Skiff")).Returns(allSkiffs);

            Dictionary<Boat, int> overlapCounts = new Dictionary<Boat, int>
            {
                { allSkiffs[0], 0 },
                { allSkiffs[1], 0 }
            };
            _reservationServiceMock.Setup(r => r.CountOverlappingActiveReservations(
                It.IsAny<List<Boat>>(), startTime, endTime)).Returns(overlapCounts);

            // Act
            bool hasOverlap = _service.SetSelectedBoat(boatId, startTime, endTime);

            // Assert
            hasOverlap.Should().BeFalse();
        }

        [Test]
        public void SetSelectedBoat_OrdersBoatsByAvailability_SelectsLeastOverlappingFirst()
        {
            // Arrange
            int boatId = 1;
            DateTime startTime = DateTime.Today.AddDays(1).AddHours(10);
            DateTime endTime = DateTime.Today.AddDays(1).AddHours(12);
            _service.AmountBoats = 2;

            Boat referenceBoat = CreateBoat("Skiff", boatId);
            Boat boat1 = CreateBoat("Skiff", 1);
            Boat boat2 = CreateBoat("Skiff", 2);
            Boat boat3 = CreateBoat("Skiff", 3);
            List<Boat> allSkiffs = new List<Boat> { boat1, boat2, boat3 };

            _boatRepoMock.Setup(r => r.Get(boatId)).Returns(referenceBoat);
            _boatRepoMock.Setup(r => r.GetAllFromName("Skiff")).Returns(allSkiffs);

            Dictionary<Boat, int> overlapCounts = new Dictionary<Boat, int>
            {
                { boat1, 2 },
                { boat2, 0 },
                { boat3, 1 }
            };
            _reservationServiceMock.Setup(r => r.CountOverlappingActiveReservations(
                It.IsAny<List<Boat>>(), startTime, endTime)).Returns(overlapCounts);

            // Act
            _service.SetSelectedBoat(boatId, startTime, endTime);
            List<Boat> selectedBoats = _service.GetCompetitionBoats();

            // Assert
            selectedBoats.Should().HaveCount(2);
            selectedBoats[0].Id.Should().Be(2);
            selectedBoats[1].Id.Should().Be(3);
        }



        [Test]
        public void CreateCompetition_WhenClientNotAuthorized_SetsReservationApprovedToFalse()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(1).AddHours(2);
            string competitionName = "Test Competition";

            Client currentClient = new Client("Organizer", "org@test.com", 3, 3, "Club", true, "hash", 1);
            Client unauthorizedClient = new Client("Participant", "part@test.com", 1, 1, "Club", true, "hash", 2);

            Boat boat = CreateBoat("Skiff", 1, 2, BoatType.S);

            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat)
            {
                TeamName = "Team A"
            };
            item.SelectedClients.Add(unauthorizedClient);

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item };

            _clientServiceMock.Setup(c => c.GetCurrentClient()).Returns(currentClient);

            Reservation createdReservation = new Reservation(DateTime.Now, startDate, endDate, 1, 1, false);
            _reservationServiceMock.Setup(r => r.CreateReservation(It.IsAny<Reservation>(), It.IsAny<List<Client>>()))
                .Returns(createdReservation);

            Competition competition = new Competition(startDate, endDate, competitionName, 1);
            _competitionRepoMock.Setup(c => c.Add(It.IsAny<Competition>())).Returns(competition);

            // Act
            Competition? result = _service.CreateCompetition(startDate, endDate, competitionName, items);

            // Assert
            result.Should().NotBeNull();
            _reservationServiceMock.Verify(r => r.CreateReservation(
                It.IsAny<Reservation>(),
                It.Is<List<Client>>(clients => clients.Contains(unauthorizedClient))), Times.Once);
        }

        [Test]
        public void CreateCompetition_WhenNoCurrentClient_ReturnsNull()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(1).AddHours(2);
            string competitionName = "Test Competition";

            _clientServiceMock.Setup(c => c.GetCurrentClient()).Returns((Client?)null);

            Boat boat = CreateBoat("Skiff", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat) { TeamName = "Team A" };
            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item };

            // Act
            Competition? result = _service.CreateCompetition(startDate, endDate, competitionName, items);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void IsClientAssignedToAnyTeam_WhenClientIsAssigned_ReturnsTrue()
        {
            // Arrange
            Client client = new Client("Test", "test@test.com", 2, 2, "Club", true, "hash", 1);
            Boat boat = CreateBoat("Skiff", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat);
            item.SelectedClients.Add(client);

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item };

            // Act
            bool result = _service.IsClientAssignedToAnyTeam(items, client.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsClientAssignedToAnyTeam_WhenClientIsNotAssigned_ReturnsFalse()
        {
            // Arrange
            Client assignedClient = new Client("Assigned", "assigned@test.com", 2, 2, "Club", true, "hash", 1);
            Boat boat = CreateBoat("Skiff", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat);
            item.SelectedClients.Add(assignedClient);

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item };

            // Act
            bool result = _service.IsClientAssignedToAnyTeam(items, 999);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsCompetitionItemComplete_WhenTeamNameAndCapacityFilled_ReturnsTrue()
        {
            // Arrange
            Boat boat = new Boat("Skiff", false, 2, 1, BoatType.S, 45, true, "Club", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat)
            {
                TeamName = "Team A"
            };
            item.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));
            item.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            // Act
            bool result = _service.IsCompetitionItemComplete(item);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsCompetitionItemComplete_WhenTeamNameEmpty_ReturnsFalse()
        {
            // Arrange
            Boat boat = new Boat("Skiff", false, 2, 1, BoatType.S, 45, true, "Club", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat)
            {
                TeamName = ""
            };
            item.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));
            item.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            // Act
            bool result = _service.IsCompetitionItemComplete(item);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsCompetitionItemComplete_WhenNotEnoughParticipants_ReturnsFalse()
        {
            // Arrange
            Boat boat = new Boat("Skiff", false, 2, 1, BoatType.S, 45, true, "Club", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat)
            {
                TeamName = "Team A"
            };
            item.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));

            // Act
            bool result = _service.IsCompetitionItemComplete(item);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsCompetitionItemComplete_WhenBoatHasSteeringWheel_IncludesExtraSeat()
        {
            // Arrange
            Boat boat = new Boat("Skiff", true, 2, 1, BoatType.S, 45, true, "Club", 1);
            BoatCompetitionUiItem item = new BoatCompetitionUiItem(boat)
            {
                TeamName = "Team A"
            };
            item.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));
            item.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            // Act
            bool result = _service.IsCompetitionItemComplete(item);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void AreAllTeamsComplete_WhenAllTeamsComplete_ReturnsTrue()
        {
            // Arrange
            Boat boat1 = new Boat("Skiff1", false, 1, 1, BoatType.S, 45, true, "Club", 1);
            Boat boat2 = new Boat("Skiff2", false, 1, 1, BoatType.S, 45, true, "Club", 2);

            BoatCompetitionUiItem item1 = new BoatCompetitionUiItem(boat1) { TeamName = "Team A" };
            item1.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));

            BoatCompetitionUiItem item2 = new BoatCompetitionUiItem(boat2) { TeamName = "Team B" };
            item2.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item1, item2 };

            // Act
            bool result = _service.AreAllTeamsComplete(items);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void AreAllTeamsComplete_WhenOneTeamIncomplete_ReturnsFalse()
        {
            // Arrange
            Boat boat1 = new Boat("Skiff1", false, 1, 1, BoatType.S, 45, true, "Club", 1);
            Boat boat2 = new Boat("Skiff2", false, 2, 1, BoatType.S, 45, true, "Club", 2);

            BoatCompetitionUiItem item1 = new BoatCompetitionUiItem(boat1) { TeamName = "Team A" };
            item1.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));

            BoatCompetitionUiItem item2 = new BoatCompetitionUiItem(boat2) { TeamName = "Team B" };
            item2.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item1, item2 };

            // Act
            bool result = _service.AreAllTeamsComplete(items);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void AreAllTeamsComplete_WhenOneTeamHasNoName_ReturnsFalse()
        {
            // Arrange
            Boat boat1 = new Boat("Skiff1", false, 1, 1, BoatType.S, 45, true, "Club", 1);
            Boat boat2 = new Boat("Skiff2", false, 1, 1, BoatType.S, 45, true, "Club", 2);

            BoatCompetitionUiItem item1 = new BoatCompetitionUiItem(boat1) { TeamName = "Team A" };
            item1.SelectedClients.Add(new Client("Client1", "c1@test.com", 2, 2, "Club", true, "hash", 1));

            BoatCompetitionUiItem item2 = new BoatCompetitionUiItem(boat2) { TeamName = "" };
            item2.SelectedClients.Add(new Client("Client2", "c2@test.com", 2, 2, "Club", true, "hash", 2));

            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem> { item1, item2 };

            // Act
            bool result = _service.AreAllTeamsComplete(items);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void AreAllTeamsComplete_WhenEmptyList_ReturnsTrue()
        {
            // Arrange
            List<BoatCompetitionUiItem> items = new List<BoatCompetitionUiItem>();

            // Act
            bool result = _service.AreAllTeamsComplete(items);

            // Assert
            result.Should().BeTrue();
        }

        private static Boat CreateBoat(string name, int id, int level = 1, BoatType type = BoatType.S)
        {
            return new Boat(name, false, 1, level, type, 45, true, "Club", id);
        }
    }
}
