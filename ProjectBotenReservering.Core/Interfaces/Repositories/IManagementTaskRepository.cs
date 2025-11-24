using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IManagementTaskRepository
{
    public ManagementTask Add(ManagementTask item);
    public ManagementTask? Get(int id);
    public List<ManagementTask> GetAll();
}

