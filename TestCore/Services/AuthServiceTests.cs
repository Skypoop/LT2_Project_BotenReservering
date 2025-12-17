using FluentAssertions;
using Moq;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace TestCore.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IClientRepository> _mockClientRepo;
    private Mock<IClientRoleRepository> _mockClientRoleRepo;
    private Mock<IRoleRepository> _mockRoleRepo;
    private AuthService _service;

    [SetUp]
    public void Setup()
    {
        _mockClientRepo = new Mock<IClientRepository>();
        _mockClientRoleRepo = new Mock<IClientRoleRepository>();
        _mockRoleRepo = new Mock<IRoleRepository>();

        _service = new AuthService(
            _mockClientRepo.Object,
            _mockClientRoleRepo.Object,
            _mockRoleRepo.Object
        );
    }

    [Test]
    public void Login_ValidCredentials_ReturnsClient()
    {
        string email = "test@test.nl";
        string password = "Password123";
        string hashedPassword = PasswordHelper.HashPassword(password);

        Client client = new Client("Test User", email, 0, 0, "TestClub", true, hashedPassword, 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        Client? result = _service.Login(email, password);

        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Test]
    public void Login_UserDoesNotExist_ReturnsNull()
    {
        string email = "unknown@test.nl";
        string password = "Password123";

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns((Client?)null);

        Client? result = _service.Login(email, password);

        result.Should().BeNull();
    }

    [Test]
    public void Login_InvalidPassword_ReturnsNull()
    {
        string email = "test@test.nl";
        string correctPassword = "Password123";
        string wrongPassword = "WrongPassword";
        string hashedPassword = PasswordHelper.HashPassword(correctPassword);

        Client client = new Client("Test User", email, 0, 0, "TestClub", true, hashedPassword, 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        Client? result = _service.Login(email, wrongPassword);

        result.Should().BeNull();
    }

    [Test]
    public void EmailExists_ClientExists_ReturnsTrue()
    {
        string email = "existing@test.nl";
        Client client = new Client("User", email, 0, 0, "Club", true, "hash", 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        bool result = _service.EmailExists(email);

        result.Should().BeTrue();
    }

    [Test]
    public void EmailExists_ClientDoesNotExist_ReturnsFalse()
    {
        string email = "new@test.nl";

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns((Client?)null);

        bool result = _service.EmailExists(email);

        result.Should().BeFalse();
    }

    [Test]
    public void GetUserRoles_ClientHasRoles_ReturnsCorrectRoles()
    {
        int clientId = 1;
        ClientRole memberRole = new ClientRole("Lid", clientId);
        ClientRole competitionCommissionerRole = new ClientRole("Wedstrijdcommissaris", clientId);
        List<ClientRole> expectedRoles = new List<ClientRole>
        {
            memberRole,
            competitionCommissionerRole
        };
        List<ClientRole> clientRoles = new List<ClientRole>
        {
            memberRole,
            competitionCommissionerRole
        };

        _mockClientRoleRepo.Setup(repo => repo.GetByClientId(clientId)).Returns(clientRoles);

        ClientRole[] result = _service.GetClientRoles(clientId);

        result.Should().Equal(expectedRoles);
    }

    [Test]
    public void GetUserRole_ClientHasNoRole_ReturnsEmptyArray()
    {
        int clientId = 1;
        ClientRole memberRole = new ClientRole("Lid", clientId);
        ClientRole competitionCommissionerRole = new ClientRole("Wedstrijdcommissaris", clientId);
        List<ClientRole> expectedRoles = new List<ClientRole>
        {
            memberRole,
            competitionCommissionerRole
        };
        List<ClientRole> emptyRoles = new List<ClientRole>();

        _mockClientRoleRepo.Setup(repo => repo.GetByClientId(clientId)).Returns(emptyRoles);

        ClientRole[] result = _service.GetClientRoles(clientId);

        result.Should().HaveCount(0);
    }

    [Test]
    public void Register_EmailAlreadyExists_ReturnsFalse()
    {
        Client client = new Client("Existing User", "existing@test.nl", 0, 0, "TestClub", true, "hash", 1);

        _mockClientRepo.Setup(repo => repo.Get(client.Email)).Returns(client);

        bool result = _service.Register(client, "password", "Lid");

        result.Should().BeFalse();
        _mockClientRepo.Verify(repo => repo.Add(It.IsAny<Client>()), Times.Never);
    }

    [Test]
    public void Register_ValidData_SavesClientAndRole()
    {
        Client client = new Client("New User", "new@test.nl", 0, 0, "TestClub", true, "hash", 10);
        string password = "SafePassword";
        string role = "Lid";

        _mockClientRepo.Setup(repo => repo.Get(client.Email)).Returns((Client?)null);

        bool result = _service.Register(client, password, role);

        result.Should().BeTrue();
        _mockClientRepo.Verify(repo => repo.Add(client), Times.Once);
        _mockClientRoleRepo.Verify(
            repo => repo.Add(It.Is<ClientRole>(cr => cr.ClientId == client.Id && cr.RoleName == role)),
            Times.Once
        );
    }
}
