using ProjectBotenReservering.Core.Models;
    
namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IReservationService
{
    public bool IsBookingWithinAllowedReservationTime(DateTime endTime);
    public bool IsValidReservationLength(DateTime startTime, DateTime endTime);
    public Task<List<Reservation>> GetAll();
}