namespace ProjectBotenReservering.Core.Models;

public class ClientManagementTask
{
    public int ClientId { get; set; }
    public int ManagementTaskId { get; set; }

    public ClientManagementTask(int clientId, int managementTaskId)
    {
        ClientId = clientId;
        ManagementTaskId = managementTaskId;
    }
}

