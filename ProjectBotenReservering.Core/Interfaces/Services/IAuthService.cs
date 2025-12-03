using ProjectBotenReservering.Core.Models;


namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Client? Login(string email, string password);
    }
}
