using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IRoleManagementTaskRepository
{
    public Task<RoleManagementTask> Add(RoleManagementTask item);
    public Task<List<RoleManagementTask>> GetByRoleId(string roleId);
    public Task<List<RoleManagementTask>> GetByManagementTaskId(int managementTaskId);
    public Task Delete(string roleId, int managementTaskId);
}

