using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IReservationService
{
    public bool IsBookingWithinAllowedReservationTime(DateTime endTime);
    public bool IsValidReservationLength(DateTime startTime, DateTime endTime);
    public Task<List<Reservation>> GetAll();
    public Reservation Add(Reservation reservation);
    public Reservation CreateReservation(Reservation reservation, List<Client> clients);
    public Reservation? Get(int id);
    public void AddClientsToReservation(Reservation reservation, List<Client> clients);
}