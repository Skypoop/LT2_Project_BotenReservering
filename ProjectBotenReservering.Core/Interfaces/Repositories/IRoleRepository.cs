using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    public Task<Role> Add(Role item);
    public Task<Role?> Get(string name);
    public Task<List<Role>> GetAll();
}

