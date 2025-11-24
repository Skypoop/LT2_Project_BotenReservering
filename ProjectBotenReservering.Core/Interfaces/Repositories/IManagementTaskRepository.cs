using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IManagementTaskRepository
{
    public Task<ManagementTask> Add(ManagementTask item);
    public Task<ManagementTask?> Get(int id);
    public Task<List<ManagementTask>> GetAll();
}

