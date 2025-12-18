using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class AuthService(IClientRepository clientRepository, IClientRoleRepository clientRoleRepository, IRoleRepository roleRepository, IRoleManagementTaskRepository roleManagementTaskRepository) : IAuthService
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IClientRoleRepository _clientRoleRepository = clientRoleRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRoleManagementTaskRepository _roleManagementTaskRepository = roleManagementTaskRepository;

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

    public bool Register(Client newClient, string password, string roleName)
    {
        Client? existingClient = _clientRepository.Get(newClient.Email);
        if (existingClient != null)
        {
            return false;
        }

        newClient.PasswordHash = PasswordHelper.HashPassword(password);

        _clientRepository.Add(newClient);
        _clientRoleRepository.Add(new ClientRole(roleName, newClient.Id));

        return true;
    }
    
    public ClientRole[] GetClientRoles(int clientId)
    {
        List<ClientRole> roles = _clientRoleRepository.GetByClientId(clientId);
        return roles.ToArray();
    }
    
    public bool CanClientUseApp(int clientId)
    {
        List<ClientRole> roles = _clientRoleRepository.GetByClientId(clientId);
        return !roles.Any(r => r.RoleName == "Gast" || r.RoleName == "Nieuw Lid");
    }
    
    public TabItem[] GetAuthorisedTabs(int clientId, TabItem[] allTabItems)
    {
        ClientRole[] roles = GetClientRoles(clientId);

        List<TabItem> accessibleTabs = new List<TabItem>();
        
        foreach (ClientRole role in roles)
        {
            List<RoleManagementTask> roleManagementTasks = _roleManagementTaskRepository.GetByRoleId(role.RoleName);

            foreach (RoleManagementTask roleManagementTask in roleManagementTasks)
            {
                accessibleTabs.Add(allTabItems[roleManagementTask.ManagementTaskId - 1]);
            }
        }

        return accessibleTabs.Distinct().ToArray();
    }
    
}