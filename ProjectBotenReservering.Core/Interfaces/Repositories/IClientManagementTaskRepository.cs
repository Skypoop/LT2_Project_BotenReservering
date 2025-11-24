using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientManagementTaskRepository
{
    public Task<ClientManagementTask> Add(ClientManagementTask item);
    public Task<List<ClientManagementTask>> GetByClientId(int clientId);
    public Task<List<ClientManagementTask>> GetByManagementTaskId(int managementTaskId);
    public Task Delete(int clientId, int managementTaskId);
}

