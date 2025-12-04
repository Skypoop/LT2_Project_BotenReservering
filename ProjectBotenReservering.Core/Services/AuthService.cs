using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class AuthService(IClientRepository clientRepository) : IAuthService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public Client? Login(string email, string password)
    {
        Client? client = _clientRepository.Get(email);
        if (client != null && PasswordHelper.VerifyPassword(password, client.PasswordHash))
        {
            return client;
        }
        return null;
    }
    public bool EmailExists(string email)
    {
        return _clientRepository.Get(email) != null;
    }

    public bool Register(Client newClient, string password)
    {
        Client? existingClient = _clientRepository.Get(newClient.Email);
        if (existingClient != null)
        {
            return false;
        }

        newClient.PasswordHash = PasswordHelper.HashPassword(password);

        _clientRepository.Add(newClient);

        return true;
    }
}