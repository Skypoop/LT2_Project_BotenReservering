using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Constants;

namespace ProjectBotenReservering.Core.Services;

public class ReservationService: IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IBoatAuthorizationService _boatAuthorizationService;
    private readonly IClientReservationRepository _clientReservationRepository;

    public ReservationService(IReservationRepository reservationRepository, IBoatAuthorizationService boatAuthorizationService, IClientReservationRepository clientReservationRepository)
    {
        _reservationRepository = reservationRepository;
        _boatAuthorizationService = boatAuthorizationService;
        _clientReservationRepository = clientReservationRepository;
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

    public void AddClientsToReservation(Reservation reservation, List<Client> clients)
    {
        foreach (Client client in clients)
        {
            ClientReservation clientReservation = new(client.Id, reservation.Id);
    
            _clientReservationRepository.Add(clientReservation);
        }
    }
}