using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IReservationRepository
{
    public Task<Reservation> Add(Reservation item);
    public Task<Reservation?> Get(int id);
    public Task<List<Reservation>> GetAll();
    public Task<List<Reservation>> GetByClientId(int clientId);
    public Task<List<Reservation>> GetByBoatId(int boatId);
}

