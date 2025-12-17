using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IReservationService _reservationService;
    private readonly ICompetitionService _competitionService;
    private readonly IClientService _clientService;
    private readonly IClientRepository _clientRepository;
    private readonly IBoatAuthorizationService _boatAuthorizationService;

    [ObservableProperty]
    public partial string TeamCount { get; set; } = "0";

    partial void OnTeamCountChanged(string value)
    {
        if (int.TryParse(value, out int teamCount))
        {
            SelectCompetitionBoatTypeIsEnable = teamCount > 1;

            _competitionService.AmountBoats = int.Parse(value);
        }
        else
        {
            SelectCompetitionBoatTypeIsEnable = false;
        }
    }

    [ObservableProperty]
    public partial ObservableCollection<Boat> CompetitionBoats { get; set; } = new ObservableCollection<Boat>();

    [ObservableProperty]
    public partial string CompetitionName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTime StartDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial TimeSpan StartTime { get; set; } = TimeSpan.Zero;

    [ObservableProperty]
    public partial DateTime EndDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial TimeSpan EndTime { get; set; } = TimeSpan.Zero;

    [ObservableProperty]
    private string teamName = string.Empty;

    [ObservableProperty]
    public partial bool SelectCompetitionBoatTypeIsEnable { get; set; } = false;

    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    private Dictionary<int, ObservableCollection<Client>> _clientsByBoatId = new Dictionary<int, ObservableCollection<Client>>();

    private readonly Dictionary<int, string> _teamNameByBoatId = new();

    [ObservableProperty]
    private Client? selectedClient;

    private ObservableCollection<Client> _selectedClients = new ObservableCollection<Client>();
    public ObservableCollection<Client> SelectedClients
    {
        get => _selectedClients;
        set => SetProperty(ref _selectedClients, value);
    }
    public ObservableCollection<Client> AvailableClients { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBoatSelected))]
    public partial Boat? SelectedBoat { get; set; }
    public bool IsBoatSelected => SelectedBoat != null;

    public CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService, IClientService clientService,
        IClientRepository clientRepository,IBoatAuthorizationService boatAuthorizationService)
    {
        _reservationService = reservationService;
        _competitionService = competitionService;
        _clientService = clientService;
        _clientRepository = clientRepository;
        _boatAuthorizationService = boatAuthorizationService;


        SelectedClients = new ObservableCollection<Client>();
        AvailableClients = new ObservableCollection<Client>();
        InitializeClients();
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        // Hasn't been implemented yet. Comment out if you want to test navigation to tweet creation.
        //if (_selectedBoats == null)
        //{
        //    return;
        //}

        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (await HandleConflictingReservationsAsync(startDateTime, endDateTime))
        {
            // Construct context string from user input for the tweet
            string contextString = $"Naam: {CompetitionName}, " +
                                   $"Datum: {StartDate:dd-MM-yyyy}, " +
                                   $"Tijd: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}, " +
                                   $"Aantal Teams: {TeamCount}";
            // TODO: Add team names to context when implemented in UI
            // Navigate to TweetCreationView and pass the context
            Dictionary<string, object> navigationParameter = new()
            {
                { "context", contextString }
            };
            // May have to be moved to popup as discussed in wireframe design
            await Shell.Current.GoToAsync(nameof(TweetCreationView), navigationParameter);
        }
    }

    [RelayCommand]
    private async Task SelectCompetitionBoatType()
    {
        await Shell.Current.GoToAsync(nameof(BoatTypeSelectionCompetitionView));
    }

    [RelayCommand]
    private void RemoveClient(Client client)
    {   
        if (client == null) return;

        if (SelectedClients.Contains(client))
        {
            SelectedClients.Remove(client);
        }

        UpdateQualificationFlags();
    }

    private async Task<bool> HandleConflictingReservationsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        List<int> boatIds = [.. (CompetitionBoats ?? Enumerable.Empty<Boat>()).Select(boat => boat.Id)];
        List<Reservation> overlappingReservations = _reservationService.FindOverlappingReservations(startDateTime, endDateTime, boatIds);

        if (overlappingReservations.Count == 0)
        {
            return true;
        }

        return await ResolveReservationConflictsAsync(overlappingReservations);
    }
    private async Task<bool> ResolveReservationConflictsAsync(List<Reservation> overlappingReservations)
    {
        bool isConfirmed = await ConfirmCancellationWithUserAsync(overlappingReservations.Count);

        if (isConfirmed)
        {
            CancelReservations(overlappingReservations);
            return true;
        }

        return false;
    }

    private static async Task<bool> ConfirmCancellationWithUserAsync(int count)
    {
        return await Shell.Current.DisplayAlert("Attentie reserveringen worden beïnvloed", $"Om ruimte te maken voor deze wedstrijd worden er {count} reserveringen geannuleerd. Tijdens het aanmaken, ga je akkoord hiermee?", "OK", "Terug");
    }

    private void CancelReservations(List<Reservation> overlappingReservations)
    {
        _reservationService.CancelOverlappingReservations(overlappingReservations);
    }

    public void FillBoatCompetitionsList()
    {
        CompetitionBoats.Clear();
        _clientsByBoatId.Clear();

        List<Boat> boats = _competitionService.GetCompetitionBoats();
        foreach (Boat boat in boats)
        {
            CompetitionBoats.Add(boat);
        }

        SelectedBoat = null;
        SelectedClients = new ObservableCollection<Client>();

        UpdateQualificationFlags();
        RefreshCompetitionCounters();
    }

    public void RefreshCompetitionCounters()
    {
        Boat boatConfig = CompetitionBoats.FirstOrDefault()!;

        if (boatConfig == null)
        {
            CalculatedBoatCount = 0;
            CalculatedPersonCount = 0;
            return;
        }

        CalculatedBoatCount = CompetitionBoats.Count;

        int capacityPerBoat = boatConfig.Seats + (boatConfig.SteeringWheel ? 1 : 0);

        CalculatedPersonCount = CalculatedBoatCount * capacityPerBoat;
    }

    private void InitializeClients()
    {
        AvailableClients.Clear();

        Client? currentUser = _clientService.GetCurrentClient();

        List<Client> allClients = _clientRepository.GetAll();
        foreach (Client client in allClients)
        {
            if (currentUser != null && client.Id == currentUser.Id) continue;
            AvailableClients.Add(client);
        }
    }

    partial void OnSelectedClientChanged(Client? value)
    {
        if (value == null) return;

        Client clientToAdd = value;
        SelectedClient = null;

        AddClientIfValid(clientToAdd);
    }

    partial void OnSelectedBoatChanged(Boat? value)
    {
        if (value == null)
        {
            SelectedClients = new ObservableCollection<Client>();
            TeamName = string.Empty;
            return;
        }

        SelectedClients = GetOrCreateClientsForBoatId(value.Id);

        TeamName = _teamNameByBoatId.TryGetValue(value.Id, out string name)
        ? name
        : string.Empty;

        UpdateQualificationFlags();
    }

    partial void OnTeamNameChanged(string value)
    {
        if (SelectedBoat == null) return;
        _teamNameByBoatId[SelectedBoat.Id] = value ?? string.Empty;
    }

    private ObservableCollection<Client> GetOrCreateClientsForBoatId(int boatId)
    {
        ObservableCollection<Client> clients;

        if (_clientsByBoatId.TryGetValue(boatId, out clients))
        {
            return clients;
        }

        clients = new ObservableCollection<Client>();
        _clientsByBoatId.Add(boatId, clients);

        Client? currentUser = _clientService.GetCurrentClient();
        if (currentUser != null)
        {
            clients.Add(currentUser);
        }

        return clients;
    }

    private void AddClientIfValid(Client clientToAdd)
    {
        if (SelectedBoat == null) return;

        int capacity = SelectedBoat.Seats;
        if (SelectedBoat.SteeringWheel)
        {
            capacity = capacity + 1;
        }

        if (SelectedClients.Count >= capacity)
        {
            _ = Shell.Current.DisplayAlert("Vol", $"De boot zit vol ({capacity} plaatsen).", "OK");
            return;
        }

        bool alreadyInBoat = SelectedClients.Any(x => x.Id == clientToAdd.Id);
        if (alreadyInBoat)
        {
            return;
        }

        SelectedClients.Add(clientToAdd);

        UpdateQualificationFlags();
    }

    private void UpdateQualificationFlags()
    {
        if (SelectedBoat == null) return;

        BoatType requiredType = SelectedBoat.Type;
        int requiredLevel = SelectedBoat.Level;

        string levelType = requiredType == BoatType.S ? "scull" : "sweep";

        foreach (Client client in SelectedClients)
        {
            bool authorized = _boatAuthorizationService.IsAuthorized(requiredType, requiredLevel, client);

            if (!authorized)
            {
                int clientLevel = requiredType == BoatType.S ? client.ScullLevel : client.SweepLevel;

                client.QualificationHelpText =
                    $"Persoon {levelType} level: {clientLevel}. Vereist: {requiredLevel}.";
                client.IsUnderqualified = true;
            }
            else
            {
                client.QualificationHelpText = string.Empty;
                client.IsUnderqualified = false;
            }
        }
    }

    [RelayCommand]
    private void ShowQualificationWarning(Client client)
    {
        string message = string.IsNullOrWhiteSpace(client.QualificationHelpText)
            ? "Persoon is te lage rang voor deze boot"
            : client.QualificationHelpText;
        Shell.Current.DisplayAlert("Waarschuwing", message, "OK");
    }

    //Call this for validation (This function checks whether one or more boats are not completely filled) Returns: Bool
    private bool AreBoatsAtFullCapacity()
    {
        foreach (Boat boat in CompetitionBoats)
        {
            int capacity = GetCapacity(boat);

            ObservableCollection<Client>? clients;
            bool hasClients = _clientsByBoatId.TryGetValue(boat.Id, out clients);

            if (!hasClients || clients == null)
            {
                return false;
            }

            if (clients.Count != capacity)
            {
                return false;
            }
        }

        return true;
    }

    private static int GetCapacity(Boat boat)
    => boat.Seats + (boat.SteeringWheel ? 1 : 0);
}