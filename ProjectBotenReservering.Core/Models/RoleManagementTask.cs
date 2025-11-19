namespace ProjectBotenReservering.Core.Models;

public class RoleManagementTask
{
    public string RoleId { get; set; }
    public int ManagementTaskId { get; set; }

    public RoleManagementTask(string roleId, int managementTaskId)
    {
        RoleId = roleId;
        ManagementTaskId = managementTaskId;
    }
}

