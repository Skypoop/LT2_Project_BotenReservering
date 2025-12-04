using ProjectBotenReservering.Core.Models;


namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Client? Login(string email, string password);
        bool Register(Client newClient, string password);
        bool EmailExists(string email);

    }
}
