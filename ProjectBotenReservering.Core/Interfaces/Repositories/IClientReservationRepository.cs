using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientReservationRepository
{
    public ClientReservation Add(ClientReservation item);
    public List<ClientReservation> GetByClientId(int clientId);
    public List<ClientReservation> GetByReservationId(int reservationId);
    public ClientReservation? Get(int clientId, int reservationId);
}

