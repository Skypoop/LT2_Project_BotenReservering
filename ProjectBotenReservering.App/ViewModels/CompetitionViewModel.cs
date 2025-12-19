using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Helpers;
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
    public partial string CompetitionName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StartTimeWithPreparation))]
    public partial DateTime StartDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StartTimeWithPreparation))]
    public partial TimeSpan StartTime { get; set; } = TimeSpan.Zero;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EndDateTime))]
    public partial DateTime EndDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EndDateTime))]
    public partial TimeSpan EndTime { get; set; } = TimeSpan.Zero;

    public DateTime EndDateTime => EndDate.Date + EndTime;
    public DateTime StartDateTime => StartDate.Date + StartTime;
    public DateTime StartTimeWithPreparation => (StartDate.Date + StartTime).AddMinutes(-30);

    [ObservableProperty]
    public partial string TeamName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool SelectCompetitionBoatTypeIsEnable { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMainEditorVisible))]
    public partial int CalculatedBoatCount { get; set; }

    public bool IsMainEditorVisible => CalculatedBoatCount > 0;

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    [ObservableProperty]
    public partial bool SubmitButtonIsEnabled { get; set; }

    [ObservableProperty]
    public partial bool HasWeatherWarning { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<BoatCompetitionUiItem> CompetitionItems { get; set; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBoatSelected))]
    [NotifyPropertyChangedFor(nameof(IsBoatNotSelected))]
    public partial BoatCompetitionUiItem? SelectedCompetitionItem { get; set; }

    public bool IsBoatSelected => SelectedCompetitionItem != null;
    public bool IsBoatNotSelected => SelectedCompetitionItem == null;

    [ObservableProperty]
    public partial Client? SelectedClient { get; set; }


    public ObservableCollection<Client> AvailableClients { get; }

    public CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService, IClientService clientService,
        IClientRepository clientRepository, IBoatAuthorizationService boatAuthorizationService)
    {
        _reservationService = reservationService;
        _competitionService = competitionService;
        _clientService = clientService;
        _clientRepository = clientRepository;
        _boatAuthorizationService = boatAuthorizationService;

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

        if (CompetitionItems.Count == 0) return;

        if (EndDateTime <= StartDateTime) return;

        WeatherAuthorizationResultEnum result = await CheckWeatherConditionsAsync(StartTimeWithPreparation, EndDateTime);

        if (result != WeatherAuthorizationResultEnum.Authorized)
        {
            SetWeatherWarning(result);
        }
    }

    private async Task<WeatherAuthorizationResultEnum> CheckWeatherConditionsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        foreach (BoatCompetitionUiItem item in CompetitionItems)
        {
            WeatherAuthorizationResultEnum result = await _boatAuthorizationService.WeatherAuthorized(item.Boat.Id, startDateTime, endDateTime);

            if (result != WeatherAuthorizationResultEnum.Authorized)
            {
                return result;
            }
        }

        return WeatherAuthorizationResultEnum.Authorized;
    }

    private void ClearWeatherWarning()
    {
        HasWeatherWarning = false;
        WeatherWarningText = string.Empty;
    }

    private void SetWeatherWarning(WeatherAuthorizationResultEnum weatherAuthorizationResult)
    {
        WeatherWarningText = MessageHelper.ConvertWeatherAuthorizationMessageToUi(weatherAuthorizationResult);

        HasWeatherWarning = true;
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        if (await CompetitionIsValid(StartTimeWithPreparation, EndDateTime) == false)
            return;

        await ValidateWeatherRulesAsync();

        if (await ShowWarningPopupAsync() && await HandleConflictingReservationsAsync(StartTimeWithPreparation, EndDateTime))
        {
            await PlaceCompetition(StartTimeWithPreparation, EndDateTime);
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
        CompetitionItems.Clear();
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
                               $"Start: {StartDate:dd-MM-yyyy} om {StartTime:hh\\:mm}, " +
                               $"Einde: {EndDate:dd-MM-yyyy} om {EndTime:hh\\:mm}, " +
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
        message += "\n- Let op: De reservering start 30 minuten eerder i.v.m. uitgifte protocol.";

        return message;
    }

    private async Task<bool> CompetitionIsValid(DateTime startDateTime, DateTime endDateTime)
    {

        List<Boat> boats = CompetitionItems.Select((BoatCompetitionUiItem x) => x.Boat).ToList();

        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, boats);

        if (!isValid)
            await Shell.Current.DisplayAlert("Fout", errorMessage, "OK");

        return isValid;
    }

    private async Task PlaceCompetition(DateTime startDateTime, DateTime endDateTime)
    {
        _competitionService.CreateCompetition(startDateTime, endDateTime, CompetitionName);

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
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        Dictionary<string, object> navigationParameter = new Dictionary<string, object>
        {
            { "start", startDateTime },
            { "end", endDateTime }
        };

        await Shell.Current.GoToAsync(nameof(BoatTypeSelectionCompetitionView), navigationParameter);
    }

    [RelayCommand]
    private void RemoveClient(Client client)
    {
        if (client == null) return;
        if (SelectedCompetitionItem == null) return;

        if (SelectedCompetitionItem.SelectedClients.Contains(client))
        {
            SelectedCompetitionItem.SelectedClients.Remove(client);
        }

        UpdateQualificationFlags();
        ValidateSubmitButton();
    }

    private async Task<bool> HandleConflictingReservationsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        List<int> boatIds = [.. (CompetitionItems ?? Enumerable.Empty<BoatCompetitionUiItem>())
                            .Select((BoatCompetitionUiItem item) => item.Boat.Id)];
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
        CompetitionItems.Clear();

        List<Boat> boats = _competitionService.GetCompetitionBoats();
        foreach (Boat boat in boats)
        {
            BoatCompetitionUiItem uiItem = new BoatCompetitionUiItem(boat);

            uiItem.PropertyChanged += (s, e) => ValidateSubmitButton();

            CompetitionItems.Add(uiItem);
        }

        SelectedCompetitionItem = null;

        UpdateQualificationFlags();
        RefreshCompetitionCounters();
    }

    public void RefreshCompetitionCounters()
    {
        CalculatedBoatCount = CompetitionItems.Count;

        BoatCompetitionUiItem? item = CompetitionItems.FirstOrDefault();
        CalculatedPersonCount = item?.Capacity * CompetitionItems.Count ?? 0;

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

    public void ValidateSubmitButton()
    {
        bool allFieldsFilled =
            !string.IsNullOrWhiteSpace(CompetitionName) &&
            CompetitionItems.Count > 0 &&
            _competitionService.AreAllTeamsComplete(CompetitionItems);

        SubmitButtonIsEnabled = allFieldsFilled;

        if (allFieldsFilled)
        {
            _ = ValidateWeatherRulesAsync();
        }
        else
        {
            ClearWeatherWarning();
        }
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

    partial void OnSelectedCompetitionItemChanged(BoatCompetitionUiItem? value)
    {
        UpdateQualificationFlags();
    }

    private void AddClientIfValid(Client clientToAdd)
    {
        if (SelectedCompetitionItem == null) return;

        if (SelectedCompetitionItem.SelectedClients.Count >= SelectedCompetitionItem.Capacity)
        {
            _ = Shell.Current.DisplayAlert("Vol", $"De boot zit vol ({SelectedCompetitionItem.Capacity} plaatsen).", "OK");
            return;
        }

        if (IsClientAlreadyInAnyBoat(clientToAdd.Id))
        {
            ShowClientIsAlreadyInABoat(clientToAdd);
            return;
        }

        SelectedCompetitionItem.SelectedClients.Add(clientToAdd);

        UpdateQualificationFlags();
        ValidateSubmitButton();
    }

    private bool IsClientAlreadyInAnyBoat(int clientId)
    {
        return _competitionService.IsClientAssignedToAnyTeam(CompetitionItems, clientId);
    }

    private void ShowClientIsAlreadyInABoat(Client client)
    {
        string message = $"{client.FullName} zit al in een andere boot";
        Shell.Current.DisplayAlert("Waarschuwing", message, "OK");
    }

    private void UpdateQualificationFlags()
    {
        if (SelectedCompetitionItem == null) return;

        BoatType requiredType = SelectedCompetitionItem.Boat.Type;
        int requiredLevel = SelectedCompetitionItem.Boat.Level;

        foreach (Client client in SelectedCompetitionItem.SelectedClients)
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

    private bool AreBoatsAtFullCapacity()
    {
        foreach (BoatCompetitionUiItem item in CompetitionItems)
        {
            if (item.SelectedClients.Count != item.Capacity)
                return false;
        }
        return true;
    }

    private bool AreAllTeamNamesFilled()
    {
        if (CompetitionItems.Count == 0)
            return false;

        foreach (BoatCompetitionUiItem item in CompetitionItems)
        {
            if (string.IsNullOrWhiteSpace(item.TeamName))
            {
                return false;
            }
        }
        return true;
    }
}