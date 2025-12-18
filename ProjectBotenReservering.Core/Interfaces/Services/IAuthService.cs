using ProjectBotenReservering.Core.Models;


namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Client? Login(string email, string password);
        bool Register(Client newClient, string password, string roleName);
        bool EmailExists(string email);
        public ClientRole[] GetClientRoles(int clientId);
        public bool CanClientUseApp(int clientId);
        public TabItem[] GetAuthorisedTabs(int clientId, TabItem[] allTabItems);

    }
}
