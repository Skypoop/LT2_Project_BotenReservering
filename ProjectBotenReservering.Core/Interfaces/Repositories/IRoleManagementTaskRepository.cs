using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IRoleManagementTaskRepository
{
    public RoleManagementTask Add(RoleManagementTask item);
    public List<RoleManagementTask> GetByRoleId(string roleId);
    public List<RoleManagementTask> GetByManagementTaskId(int managementTaskId);
    public void Delete(string roleId, int managementTaskId);
}

