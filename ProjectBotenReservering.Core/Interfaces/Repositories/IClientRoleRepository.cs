using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientRoleRepository
{
    public ClientRole Add(ClientRole item);
    public List<ClientRole> GetByClientId(int clientId);
    public List<ClientRole> GetByRoleName(string roleName);
    public void Delete(string roleName, int clientId);
}

