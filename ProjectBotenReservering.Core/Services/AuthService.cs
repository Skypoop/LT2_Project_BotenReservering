using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Helpers; 

namespace ProjectBotenReservering.Core.Services
{
    public class AuthService(IClientRepository clientRepository) : IAuthService
    {
        public Client? Login(string email, string password)
        {
            Client client = clientRepository.Get(email);

            if (client != null && PasswordHelper.VerifyPassword(password, client.PasswordHash))
            {
                return client;
            }

            return null;
        }
    }
}