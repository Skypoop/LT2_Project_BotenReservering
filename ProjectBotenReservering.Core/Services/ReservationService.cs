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
        _reservationRepository = reservationRepository;
        _boatAuthorizationService = boatAuthorizationService;
        
    }

    public Reservation Add(Reservation reservation)
    {
        return _reservationRepository.Add(reservation);
    }
    
    public Reservation? Get(int id)
    {
        return _reservationRepository.Get(id);
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
        if (timeDifference.TotalMinutes > ReservationRules.MaxReservationLength)
        {
            return false;
        }
        return true;
    }

    public bool IsReservationTimeFree(DateTime startTime, DateTime endTime)
    {
        throw new NotImplementedException();
    }
}