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
    public partial string TeamName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool SelectCompetitionBoatTypeIsEnable { get; set; } = false;

    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    [ObservableProperty]
    public partial bool SubmitButtonIsEnabled { get; set; }

    [ObservableProperty]
    public partial bool HasWeatherWarning { get; set; }

    private Dictionary<int, ObservableCollection<Client>> _clientsByBoatId = new Dictionary<int, ObservableCollection<Client>>();

    private readonly Dictionary<int, string> _teamNameByBoatId = new();

    [ObservableProperty]
    public partial Client? SelectedClient { get; set; }

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
        IClientRepository clientRepository, IBoatAuthorizationService boatAuthorizationService)
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

    [ObservableProperty] public partial string? WeatherWarningText { get; set; }

    partial void OnStartDateChanged(DateTime value) => _ = ValidateWeatherRulesAsync();
    partial void OnStartTimeChanged(TimeSpan value) => _ = ValidateWeatherRulesAsync();
    partial void OnEndDateChanged(DateTime value) => _ = ValidateWeatherRulesAsync();
    partial void OnEndTimeChanged(TimeSpan value) => _ = ValidateWeatherRulesAsync();

    private async Task ValidateWeatherRulesAsync()
    {
        ClearWeatherWarning();

        if (CompetitionBoats.Count == 0) return;

        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (endDateTime <= startDateTime) return;

        bool hasWeatherIssues = await CheckWeatherConditionsAsync(startDateTime, endDateTime);

        if (hasWeatherIssues)
        {
            SetWeatherWarning();
        }
    }

    private async Task<bool> CheckWeatherConditionsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        foreach (Boat boat in CompetitionBoats)
        {
            bool weatherAllowed = await _boatAuthorizationService.WeatherAuthorized(boat.Id, startDateTime, endDateTime);

            if (!weatherAllowed)
            {
                return true;
            }
        }

        return false;
    }

    private void ClearWeatherWarning()
    {
        HasWeatherWarning = false;
        WeatherWarningText = string.Empty;
    }

    private void SetWeatherWarning()
    {
        WeatherWarningText = "LET OP: Voor deze datum en tijd is het weer heftig voor een of meerdere geselecteerde boten!";
        HasWeatherWarning = true;
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (await CompetitionIsValid() == false)
            return;

        await ValidateWeatherRulesAsync();

        if (await ShowWarningPopupAsync() && await HandleConflictingReservationsAsync(startDateTime, endDateTime))
        {
            await PlaceCompetition();
        }
    }

    private void RefreshScreen()
    {
        CompetitionName = String.Empty;
        StartDate = DateTime.Today;
        StartTime = TimeSpan.Zero;
        EndDate = DateTime.Today;
        EndTime = TimeSpan.Zero;
        TeamCount = "0";
        _competitionService.AmountBoats = 0;
        _competitionService.ClearCompetitionBoats();
        CompetitionBoats.Clear();
        RefreshCompetitionCounters();
    }

    private async Task<bool> ShowWarningPopupAsync()
    {
        string message = CreateConfirmationPopupMessage();
        return await Shell.Current.DisplayAlert("Bevestigen", message, "Bevestigen", "Terug");
    }

    private async Task MoveToTweetScreen()
    {
        // Construct context string from user input for the tweet
        string contextString = $"Naam: {CompetitionName}, " +
                               $"Datum: {StartDate:dd-MM-yyyy}, " +
                               $"Tijd: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}, " +
                               $"Aantal Teams: {TeamCount}";
        // TO-DO: Add team names to context when implemented in UI
        // Navigate to TweetCreationView and pass the context
        Dictionary<string, object> navigationParameter = new()
        {
            { "context", contextString }
        };

        await Shell.Current.GoToAsync(nameof(TweetCreationView), navigationParameter);
    }

    private string CreateConfirmationPopupMessage()
    {
        string message = "Waarschuwingen:\n";

        if (HasWeatherWarning)
        {
            message += $"- {WeatherWarningText}\n";
        }

        message += "\n- De wedstrijd zal worden aangemaakt met de opgegeven gegevens.";

        return message;
    }

    private async Task<bool> CompetitionIsValid()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;


        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, CompetitionBoats.ToList());

        if (!isValid)
            await Shell.Current.DisplayAlert("Fout", errorMessage, "OK");

        return isValid;
    }

    private async Task PlaceCompetition()
    {
        _competitionService.CreateCompetition(StartDate + StartTime, EndDate + EndTime, CompetitionName);

        if (await ShowCompletionMessage())
            await MoveToTweetScreen();
        else
            RefreshScreen();

    }

    private async Task<bool> ShowCompletionMessage()
    {
        return await Shell.Current.DisplayAlert("Wedstrijd Aangemaakt", "De wedstrijd is succesvol aangemaakt.\nWil je een tweet aanmaken voor social media?", "Ja", "Nee");
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
        List<Reservation> overlappingReservations =
            _reservationService.FindOverlappingReservations(startDateTime, endDateTime, boatIds);

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
        return await Shell.Current.DisplayAlert("Attentie: reserveringen worden beïnvloed",
            $"Om ruimte te maken voor deze wedstrijd worden er {count} reserveringen geannuleerd. Ga je hiermee akkoord?",
            "OK", "Terug");
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
        CalculatedBoatCount = CompetitionBoats.Count;

        Boat? boat = CompetitionBoats.FirstOrDefault();
        CalculatedPersonCount = boat?.Seats * CompetitionBoats.Count ?? 0;

        _ = ValidateWeatherRulesAsync();
    }

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

    partial void OnCompetitionNameChanged(string value)
    {
        ValidateSubmitButton();
    }

    partial void OnCalculatedBoatCountChanged(int value)
    {
        ValidateSubmitButton();
    }

    partial void OnTeamNameChanged(string oldValue, string newValue)
    {
        ValidateSubmitButton();
    }

    public void ValidateSubmitButton()
    {
        SubmitButtonIsEnabled =
        !string.IsNullOrWhiteSpace(CompetitionName) &&
        CompetitionBoats.Count > 0 &&
        AreAllTeamNamesFilled();

        // Additional validations for the submit button to be enabled should be added here

        _ = ValidateWeatherRulesAsync();
    }

    private void InitializeClients()
    {
        AvailableClients.Clear();

        List<Client> allClients = _clientRepository.GetAll();
        foreach (Client client in allClients)
        {
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
        if (value is null)
        {
            SelectedClients = new ObservableCollection<Client>();
            TeamName = string.Empty;
            return;
        }

        ObservableCollection<Client> clients = GetOrCreateClientsForBoatId(value.Id);
        SelectedClients = clients ?? new ObservableCollection<Client>();

        string? name;
        bool hasName = _teamNameByBoatId.TryGetValue(value.Id, out name);

        TeamName = (hasName && !string.IsNullOrWhiteSpace(name))
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
        ObservableCollection<Client>? clients;

        if (_clientsByBoatId.TryGetValue(boatId, out clients) && clients != null)
        {
            return clients;
        }

        clients = new ObservableCollection<Client>();
        _clientsByBoatId[boatId] = clients;

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

        if (IsClientAlreadyInAnyBoat(clientToAdd.Id))
        {
            ShowClientIsAlreadyInABoat(clientToAdd);
            return;
        }
        
        SelectedClients.Add(clientToAdd);

        UpdateQualificationFlags();
    }

    private bool IsClientAlreadyInAnyBoat(int clientId)
    {
        foreach (Boat boat in CompetitionBoats)
        {
            if (!_clientsByBoatId.TryGetValue(boat.Id, out ObservableCollection<Client>? clients) || clients == null)
                continue;

            if (clients.Any(c => c.Id == clientId))
                return true;
        }

        return false;
    }

    private void ShowClientIsAlreadyInABoat(Client client)
    {
        string message = $"{client.FullName} zit al in een andere boot";
        Shell.Current.DisplayAlert("Waarschuwing", message, "OK");
    }

    private void UpdateQualificationFlags()
    {
        if (SelectedBoat == null) return;

        BoatType requiredType = SelectedBoat.Type;
        int requiredLevel = SelectedBoat.Level;

        foreach (Client client in SelectedClients)
        {
            ApplyQualificationState(client, requiredType, requiredLevel);
        }
    }

    private void ApplyQualificationState(Client client, BoatType requiredType, int requiredLevel)
    {
        bool authorized = _boatAuthorizationService.IsAuthorized(requiredType, requiredLevel, client);

        if (authorized)
        {
            ClearQualification(client);
            return;
        }

        SetUnderqualified(client, requiredType, requiredLevel);
    }

    private static void ClearQualification(Client client)
    {
        client.QualificationHelpText = string.Empty;
        client.IsUnderqualified = false;
    }

    private static void SetUnderqualified(Client client, BoatType requiredType, int requiredLevel)
    {
        int clientLevel = requiredType == BoatType.S ? client.ScullLevel : client.SweepLevel;
        string levelType = requiredType == BoatType.S ? "scull" : "sweep";

        client.QualificationHelpText =
            $"Persoon {levelType} level: {clientLevel}. Vereist: {requiredLevel}.";
        client.IsUnderqualified = true;
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

    private bool AreAllTeamNamesFilled()
    {
        if (CompetitionBoats.Count == 0)
            return false;

        foreach (Boat boat in CompetitionBoats)
        {
            if (!_teamNameByBoatId.TryGetValue(boat.Id, out string? name) ||
                string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
        }

        return true;
    }

    private static int GetCapacity(Boat boat) => boat.Seats + (boat.SteeringWheel ? 1 : 0);
}