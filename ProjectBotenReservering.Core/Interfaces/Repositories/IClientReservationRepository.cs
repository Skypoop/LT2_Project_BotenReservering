using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientReservationRepository
{
    public Task<ClientReservation> Add(ClientReservation item);
    public Task<List<ClientReservation>> GetByClientId(int clientId);
    public Task<List<ClientReservation>> GetByReservationId(int reservationId);
    public Task<ClientReservation>? Get(int clientId, int reservationId);
    public Task UpdateApproval(int clientId, int reservationId, bool approved);
}

