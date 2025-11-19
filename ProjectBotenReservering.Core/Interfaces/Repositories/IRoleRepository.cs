using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    public Role Add(Role item);
    public Role? Get(string name);
    public List<Role> GetAll();
}

