using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IBoatRepository
{
    public Task<Boat> Add(Boat item);
    public Task<Boat>? Get(int id);
    public Task<List<Boat>> GetAll();
    public Task<List<Boat>> GetOperationalBoats();
    public Task Delete(int boatId);
}

