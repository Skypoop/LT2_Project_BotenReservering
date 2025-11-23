using ProjectBotenReservering.Core.Models;
    
namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IReservationService
{
    public List<Reservation> GetUpcomingReservations();
    public Reservation AddReservation(Reservation  reservation);
    public bool IsBookingWithinAllowedReservationTime(DateTime startTime, DateTime endTime);
    public bool IsValidReservationLength(DateTime startTime, DateTime endTime);
    public bool IsReservationTimeFree(DateTime startTime, DateTime endTime);
}