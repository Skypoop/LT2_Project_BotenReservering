using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IReservationRepository
{
    Reservation Add(Reservation item);
    Reservation? Get(int id);
    List<Reservation> GetAll();
    List<Reservation> GetByClientId(int clientId);
    List<Reservation> GetByBoatId(int boatId);
}

