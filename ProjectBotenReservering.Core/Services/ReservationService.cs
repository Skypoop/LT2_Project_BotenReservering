using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Constants;

namespace ProjectBotenReservering.Core.Services;

public class ReservationService: IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IBoatAuthorizationService _boatAuthorizationService;
    private readonly IClientReservationRepository _clientReservationRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        IBoatAuthorizationService boatAuthorizationService,
        IClientReservationRepository clientReservationRepository
    )
    {
        _reservationRepository = reservationRepository;
        _boatAuthorizationService = boatAuthorizationService;
        _clientReservationRepository = clientReservationRepository;
    }

    public Reservation Add(Reservation reservation)
    {
        return _reservationRepository.Add(reservation);
    }

    public Reservation CreateReservation(Reservation reservation, List<Client> clients)
    {
        bool allAuthorized = true;
        foreach (Client client in clients)
        {
            if (!_boatAuthorizationService.IsAuthorized(reservation.BoatId, client))
            {
                allAuthorized = false;
                break;
            }
        }
        reservation.Approved = allAuthorized;
        Add(reservation);
        AddClientsToReservation(reservation, clients);
        return reservation;
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

    public async Task<List<Reservation>> GetAll()
    {
        return _reservationRepository.GetAll();
    }
    
    public bool IsReservationTimeBlocked(IEnumerable<Reservation> reservations, DateTime startTime, DateTime endTime, BoatTypeUiItem boatType)
    {
        float[][] existingTimes = reservations
            .Select(r => IntervalHelper.TimeSlotToInterval(r.StartTime, r.EndTime))
            .ToArray();        
            float[] enteredTimes = IntervalHelper.TimeSlotToInterval(startTime, endTime);

            return IntervalHelper.CountIntersectionsWithIntervalList(enteredTimes, existingTimes) > boatType.Amount;
    }

    public void AddClientsToReservation(Reservation reservation, List<Client> clients)
    {
        foreach (Client client in clients)
        {
            ClientReservation clientReservation = new(client.Id, reservation.Id);
    
            _clientReservationRepository.Add(clientReservation);
        }
    }
}