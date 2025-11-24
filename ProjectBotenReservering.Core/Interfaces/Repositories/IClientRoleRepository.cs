using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientRoleRepository
{
    public Task<ClientRole> Add(ClientRole item);
    public Task<List<ClientRole>> GetByClientId(int clientId);
    public Task<List<ClientRole>> GetByRoleName(string roleName);
    public Task Delete(string roleName, int clientId);
}

