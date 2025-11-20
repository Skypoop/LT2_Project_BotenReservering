using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientManagementTaskRepository
{
    public ClientManagementTask Add(ClientManagementTask item);
    public List<ClientManagementTask> GetByClientId(int clientId);
    public List<ClientManagementTask> GetByManagementTaskId(int managementTaskId);
    public void Delete(int clientId, int managementTaskId);
}

