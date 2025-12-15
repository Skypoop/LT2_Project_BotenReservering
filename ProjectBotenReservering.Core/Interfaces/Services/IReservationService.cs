using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IReservationService
{
    public bool IsBookingWithinAllowedReservationTime(DateTime endTime);
    public bool IsValidReservationLength(DateTime startTime, DateTime endTime);
    public List<Reservation> GetAll();
    public Reservation Add(Reservation reservation);
    public Reservation CreateReservation(Reservation reservation, List<Client> clients);
    public Reservation? Get(int id);
    public void AddClientsToReservation(Reservation reservation, List<Client> clients);
    public bool IsReservationTimeBlocked(IEnumerable<Reservation> reservations, DateTime startTime, DateTime endTime, BoatTypeUiItem boatType);
    public void CancelOverlappingReservations(List<Reservation> reserservations);
    public List<Reservation> FindOverlappingReservations(DateTime startDate, DateTime endDate, List<int> boatIds);
}