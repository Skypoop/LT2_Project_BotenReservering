using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Constants;

namespace ProjectBotenReservering.Core.Services;

public class ReservationService: IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IBoatAuthorizationService _boatAuthorizationService;
    public ReservationService(IReservationRepository reservationRepository, IBoatAuthorizationService boatAuthorizationService)
    {
        this._reservationRepository = reservationRepository;
        this._boatAuthorizationService = boatAuthorizationService;
        
    }
    
    public bool IsBookingWithinAllowedReservationTime(DateTime startTime)
    {
        DateTime today = DateTime.Today;
        TimeSpan daysFromNow =  startTime.Subtract(today);
        if (daysFromNow.Days > ReservationRules.MaxDaysBeforeReservation)
        {
            return false;
        }
        return true;
    }

    public bool IsValidReservationLength(DateTime startTime, DateTime endTime)
    {
        TimeSpan timeDifference = endTime - startTime;
        if (timeDifference.Hours > ReservationRules.MaxReservationLength)
        {
            return false;
        }
        return true;
    }

    public async Task<List<Reservation>> GetAll()
    {
        return _reservationRepository.GetAll();
    }
    
    public bool IsReservationTimeFree(DateTime startTime, DateTime endTime)
    {
        throw new NotImplementedException();
    }
}