using Moq;
using NUnit.Framework;
using FluentAssertions;
using ProjectBotenReservering.Core.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Helpers;
using System.Collections.Generic;

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
    public void Login_ReturnsClient_WhenCredentialsAreValid()
    {
        // Arrange
        string email = "test@test.nl";
        string password = "Password123";
        string hashedPassword = PasswordHelper.HashPassword(password);

        Client client = new Client("Test User", email, 0, 0, "TestClub", true, hashedPassword, 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        // Act
        Client? result = _service.Login(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Test]
    public void Login_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        string email = "unknown@test.nl";
        string password = "Password123";

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns((Client?)null);

        // Act
        Client? result = _service.Login(email, password);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Login_ReturnsNull_WhenPasswordIsInvalid()
    {
        // Arrange
        string email = "test@test.nl";
        string correctPassword = "Password123";
        string wrongPassword = "WrongPassword";
        string hashedPassword = PasswordHelper.HashPassword(correctPassword);

        Client client = new Client("Test User", email, 0, 0, "TestClub", true, hashedPassword, 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        // Act
        Client? result = _service.Login(email, wrongPassword);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void EmailExists_ReturnsTrue_WhenClientExists()
    {
        // Arrange
        string email = "existing@test.nl";
        Client client = new Client("User", email, 0, 0, "Club", true, "hash", 1);

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns(client);

        // Act
        bool result = _service.EmailExists(email);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EmailExists_ReturnsFalse_WhenClientDoesNotExist()
    {
        // Arrange
        string email = "new@test.nl";

        _mockClientRepo.Setup(repo => repo.Get(email)).Returns((Client?)null);

        // Act
        bool result = _service.EmailExists(email);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetUserRole_ReturnsCorrectRole_WhenClientExists()
    {
        // Arrange
        int clientId = 1;
        string expectedRole = "Lid";
        List<ClientRole> clientRoles = new List<ClientRole>
        {
            new ClientRole(expectedRole, clientId)
        };

        _mockClientRoleRepo.Setup(repo => repo.GetByClientId(clientId)).Returns(clientRoles);

        // Act
        string result = _service.GetUserRole(clientId);

        // Assert
        result.Should().Be(expectedRole);
    }

    [Test]
    public void GetUserRole_ReturnsEmptyString_WhenClientHasNoRole()
    {
        // Arrange
        int clientId = 1;
        List<ClientRole> emptyRoles = new List<ClientRole>();

        _mockClientRoleRepo.Setup(repo => repo.GetByClientId(clientId)).Returns(emptyRoles);

        // Act
        string result = _service.GetUserRole(clientId);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Test]
    public void Register_ReturnsFalse_WhenEmailAlreadyExists()
    {
        // Arrange
        Client client = new Client("Existing User", "existing@test.nl", 0, 0, "TestClub", true, "hash", 1);

        _mockClientRepo.Setup(repo => repo.Get(client.Email)).Returns(new Client("Existing User", "existing@test.nl", 0, 0, "TestClub", true, "hash", 1));

        // Act
        bool result = _service.Register(client, "password", "Lid");

        // Assert
        result.Should().BeFalse();
        _mockClientRepo.Verify(repo => repo.Add(It.IsAny<Client>()), Times.Never);
    }

    [Test]
    public void Register_SavesClientAndRole_WhenDataIsValid()
    {
        // Arrange
        Client client = new Client("New User", "new@test.nl", 0, 0, "TestClub", true, "hash", 10);
        string password = "SafePassword";
        string role = "Lid";

        _mockClientRepo.Setup(repo => repo.Get(client.Email)).Returns((Client?)null);

        // Act
        bool result = _service.Register(client, password, role);

        // Assert
        result.Should().BeTrue();
        _mockClientRepo.Verify(repo => repo.Add(client), Times.Once);
        _mockClientRoleRepo.Verify(repo => repo.Add(It.Is<ClientRole>(cr => cr.ClientId == client.Id && cr.RoleName == role)), Times.Once);
    }
}