using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Data.Repositories;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IReservationService _reservationService;
    private readonly ICompetitionService _competitionService;
    private readonly IClientService _clientService;
    private readonly IClientRepository _clientRepository;

    private string _teamCount = "0";

    public string TeamCount
    {
        get => _teamCount;
        set
        {
            if (SetProperty(ref _teamCount, value))
            {
                if (CheckBoatAmountIsValid(value))
                {
                    SelectCompetitionBoatTypeIsEnable = true;
                    _competitionService.AmountBoats = int.Parse(value);
                }
                else
                {
                    SelectCompetitionBoatTypeIsEnable = false;
                }
            }
        }
    }

    [ObservableProperty]
    public ObservableCollection<Boat> competitionBoats = new ObservableCollection<Boat>();

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
        IClientRepository clientRepository)
    {
        _reservationService = reservationService;
        _competitionService = competitionService;
        _clientService = clientService;
        _clientRepository = clientRepository;

        SelectedClients = new ObservableCollection<Client>();
        AvailableClients = new ObservableCollection<Client>();
        InitializeClients();
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, competitionBoats.ToList());

        if (!isValid)
        {
            await Shell.Current.DisplayAlert("Fout", errorMessage, "OK");
            return;
        }

        if (await ReservationsNotOverlappingWithTheCompetition(startDateTime, endDateTime))
        {
            //Make competition function
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
        Client? currentUser = _clientService.GetCurrentClient();
        
        if (client == null) return;

        if (SelectedClients.Contains(client))
        {
            SelectedClients.Remove(client);
        }

        UpdateQualificationFlags();
    }

    private async Task<bool> ReservationsNotOverlappingWithTheCompetition(DateTime startDateTime, DateTime endDateTime)
    {
        List<Reservation> overlappingReservations = _reservationService.FindOverlappingReservations(startDateTime, endDateTime, competitionBoats.Select(b => b.Id).ToList());

        if (overlappingReservations.Count > 0)
        {
            return await ShowWarningOverlappingReservationsDialog(overlappingReservations);
        }

        return false;
    }

    private async Task<bool> ShowWarningOverlappingReservationsDialog(List<Reservation> overlappingReservations)
    {
        bool answer = await Shell.Current.DisplayAlert("Attentie reserveringen worden beïnvloed", $"Om ruimte te maken voor deze wedstrijd worden er {overlappingReservations.Count} reserveringen geannuleerd. Tijdens het aanmaken, ga je akkoord hiermee?", "OK", "Terug");

        if (answer)
        {
            CancelOverlappingReservations(overlappingReservations);

            return true;
        }

        return false;
    }

    private void CancelOverlappingReservations(List<Reservation> overlappingReservations)
    {
        _reservationService.CancelOverlappingReservations(overlappingReservations);
    }

    private bool CheckBoatAmountIsValid(string boatAmount)
    {
        if (string.IsNullOrWhiteSpace(boatAmount))
        {
            return false;
        }

        if (int.TryParse(boatAmount, out int amount))
        {
            return amount > 1;
        }

        return false;
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

        RefreshCompetitionCounters(boats);

        SelectedBoat = null;
        SelectedClients = new ObservableCollection<Client>();

        UpdateQualificationFlags();
    }

    public void RefreshCompetitionCounters(List<Boat> boats)
    {
        if (boats != null && boats.Count > 0)
        {
            CalculatedBoatCount = CompetitionBoats.Count;

            if (boats[0].SteeringWheel)
            {
                CalculatedPersonCount = CompetitionBoats.Count * (boats.FirstOrDefault().Seats + 1);
            }
            else
            {
                CalculatedPersonCount = CompetitionBoats.Count * boats.FirstOrDefault().Seats;
            }
        }
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
        ObservableCollection<Client> Clients;

        if (_clientsByBoatId.TryGetValue(boatId, out Clients))
        {
            return Clients;
        }

        Clients = new ObservableCollection<Client>();
        _clientsByBoatId.Add(boatId, Clients);

        Client? currentUser = _clientService.GetCurrentClient();
        if (currentUser != null)
        {
            Clients.Add(currentUser);
        }

        return Clients;
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

        int requiredLevel = SelectedBoat.Level;

        BoatType requiredType = SelectedBoat.Type;
        bool isScull = requiredType == BoatType.S;

        string levelType = isScull ? "scull" : "sweep";

        foreach (Client client in SelectedClients)
        {
            int clientLevel = isScull ? client.ScullLevel : client.SweepLevel;

            bool authorized = clientLevel >= requiredLevel;

            if (!authorized)
            {
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
}