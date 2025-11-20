namespace ProjectBotenReservering.Core.Models;

public class ClientRole
{
    public string RoleName { get; set; }
    public int ClientId { get; set; }

    public ClientRole(string roleName, int clientId)
    {
        RoleName = roleName;
        ClientId = clientId;
    }
}

