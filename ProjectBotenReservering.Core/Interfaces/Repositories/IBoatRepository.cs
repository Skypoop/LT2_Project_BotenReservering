using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IBoatRepository
{
    public Boat Add(Boat item);
    public Boat? Get(int id);
    public List<Boat> GetAll();
    public List<Boat> GetOperationalBoats();
}

