using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionService : ICompetitionService
{
    private readonly IBoatRepository _boatRepository;
    private readonly ICompetitionRepository _competitionRepository;
    private readonly IReservationService _reservationService;
    private readonly IReservationCompetitionRepository _reservationCompetitionRepository;
    private readonly IClientService _clientService;

    private readonly List<Boat> _competitionBoatsList = [];

    private int _selectedBoatId;
    private int _amountBoats;

    public int SelectedBoatId
    {
        get { return _selectedBoatId; }
        private set { _selectedBoatId = value; }
    }

    public int AmountBoats
    {
        get { return _amountBoats; }
        set { _amountBoats = value; }
    }

    public CompetitionService(IReservationService reservationService, IClientService clientService, IBoatRepository boatRepository, ICompetitionRepository competitionRepository, IReservationCompetitionRepository reservationCompetitionRepository)
    {
        _reservationService = reservationService;
        _clientService = clientService;
        _boatRepository = boatRepository;
        _competitionRepository = competitionRepository;
        _reservationCompetitionRepository = reservationCompetitionRepository;
    }

    public void ClearCompetitionBoats()
    {
        _competitionBoatsList.Clear();
    }

    public bool HasEnoughBoats(int boatId)
    {
        if (_boatRepository.Get(boatId) == null)
        {
            return false;
        }

        List<Boat> allBoatsFromName = GetAllCompetitionBoatsFromName(boatId);

        return allBoatsFromName.Count >= AmountBoats;
    }
    public bool SetSelectedBoat(int boatId, DateTime startTime, DateTime endTime)
    {
        _competitionBoatsList.Clear();

        List<Boat> allBoatsFromName = GetAllCompetitionBoatsFromName(boatId);
        allBoatsFromName = OrderBoatListByAvailability(allBoatsFromName, startTime, endTime);
        
        int countToAdd = Math.Min(allBoatsFromName.Count, AmountBoats);
        
        for (int i = 0; i < countToAdd; i++)
        {
            _competitionBoatsList.Add(allBoatsFromName[i]);
        }

        SelectedBoatId = boatId;
        
        bool hasAnyOverlap = _reservationService.CountOverlappingActiveReservations(_competitionBoatsList, startTime, endTime).Values.Any(v => v != 0);
        return hasAnyOverlap;
    }
    
    private List<Boat> OrderBoatListByAvailability(List<Boat> boats, DateTime startDateTime, DateTime endDateTime)
    {
        Dictionary<Boat, int> boatAvailability = _reservationService.CountOverlappingActiveReservations(boats, startDateTime, endDateTime);
        
        return boatAvailability.OrderBy(b => b.Value).Select(b=> b.Key).ToList();
    }
    
    private List<Boat> GetAllCompetitionBoatsFromName(int id)
    {
        Boat? boat = _boatRepository.Get(id);
        if (boat == null)
            throw new ArgumentException($"Boat with ID {id} does not exist.", nameof(id));

        if (string.IsNullOrWhiteSpace(boat.Name))
            return new List<Boat>();

        return _boatRepository.GetAllFromName(boat.Name) ?? new List<Boat>();
    }

    public Competition? CreateCompetition(DateTime startDate, DateTime endDate, string competitionName)
    {
        List<Reservation> reservations = new();
        Client? currentClient = _clientService.GetCurrentClient();

        if (currentClient == null)
        {
            Console.WriteLine($"Logged in as invalid client at {nameof(CreateCompetition)}");
            return null;
        }

        foreach (Boat boat in _competitionBoatsList)
        {
            List<Client> clients = new(); // IMPLEMENT CLIENTS PER BOAT HERE

            Reservation res = _reservationService.CreateReservation(new Reservation(
                DateTime.Now,
                startDate,
                endDate,
                currentClient.Id,
                boat.Id,
                true), clients);

            reservations.Add(res);
        }

        Competition competition = _competitionRepository.Add(new Competition(startDate, endDate, competitionName));
        foreach (Reservation reservation in reservations)
        {
            // TO-DO GET TEAM NAMES FROM UI
            ReservationCompetition reservationCompetition = new(competition.Id, reservation.Id, "NO TEAM NAME");
            _reservationCompetitionRepository.Add(reservationCompetition);
        }

        return competition;
    }

    public List<Boat> GetCompetitionBoats()
    {
        return _competitionBoatsList;
    }
    public (bool IsValid, string? ErrorMessage) ValidateCompetition(DateTime start, DateTime end, List<Boat> boats)
    {
        if (!CompetitionValidationHelper.IsCompetitionEndDateValid(start, end))
        {
            return (false, "De einddatum moet later zijn dan de begindatum.");
        }

        if (!CompetitionValidationHelper.IsCompetitionStartDateValid(start))
        {
            return (false, "De begindatum mag niet in het verleden liggen.");
        }

        if (!CompetitionValidationHelper.AreBoatsSelected(boats))
        {
            return (false, "Er zijn geen boten geselecteerd.");
        }

        return (true, null);
    }

    public bool IsCompetitionItemComplete(BoatCompetitionUiItem item)
    {
        return !string.IsNullOrWhiteSpace(item.TeamName) &&
               item.SelectedClients.Count == item.Capacity;
    }

    public bool AreAllTeamsComplete(IEnumerable<BoatCompetitionUiItem> items)
    {
        return items.All(item => IsCompetitionItemComplete(item));
    }

    public bool IsClientAssignedToAnyTeam(IEnumerable<BoatCompetitionUiItem> items, int clientId)
    {
        return items.Any(item => item.SelectedClients.Any(c => c.Id == clientId));
    }
}