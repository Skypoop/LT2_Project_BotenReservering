using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Calendar.Models;
using ProjectBotenReservering.App.Helpers;
using ProjectBotenReservering.Core.Constants;
using ProjectBotenReservering.Core.Interfaces.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

[QueryProperty(nameof(BoatId), "Id")]
public partial class ReservationFormViewModel : BaseViewModel
{
    private readonly IBoatTypeService _boatTypeService;
    private readonly IClientService _clientService;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationService _reservationService;
    private readonly IBoatAuthorizationService _boatAuthorizationService;
    private readonly ISmtpMailService _mailService;
    private readonly IResourceLoader _resourceLoader;

    public ReservationFormViewModel(
        IBoatTypeService boatTypeService,
        IClientService clientService,
        IClientRepository clientRepository,
        IReservationService reservationService,
        IBoatAuthorizationService boatReservationService,
        ISmtpMailService mailservice,
        IResourceLoader resourceLoader
        )
    {
        _boatTypeService = boatTypeService;
        _clientService = clientService;
        _clientRepository = clientRepository;
        _reservationService = reservationService;
        _boatAuthorizationService = boatReservationService;
        _mailService = mailservice;
        _resourceLoader = resourceLoader;

        Title = "";

        SelectedDate = DateTime.Today;
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(10, 0, 0);

        SelectedClients = new ObservableCollection<Client>();
        AvailableClients = new ObservableCollection<Client>();

        MinDate = DateTime.Today;
    }

    private DateTime? _minimumDate;
    public DateTime MinDate
    {
        get => _minimumDate ?? DateTime.Today;
        set
        {
            _minimumDate = value;
            OnPropertyChanged();

        }
    }

    private ObservableCollection<Reservation> ReservationList { get; set; } = new ObservableCollection<Reservation>();

    [ObservableProperty]
    public partial BoatTypeUiItem? CurrentBoatType { get; set; }
    [ObservableProperty]
    public partial DateTime SelectedDate { get; set; }
    [ObservableProperty]
    public partial TimeSpan StartTime { get; set; }
    [ObservableProperty]
    public partial TimeSpan EndTime { get; set; }
    [ObservableProperty]
    public partial string? DateWarningText { get; set; }
    [ObservableProperty]
    public partial bool HasWeatherWarning { get; set; }
    [ObservableProperty]
    public partial bool HasDateWarning { get; set; }
    [ObservableProperty]
    public partial string? TimeWarningText { get; set; }
    [ObservableProperty]
    public partial bool HasTimeWarning { get; set; }

    public ObservableCollection<Client> SelectedClients { get; }
    public ObservableCollection<Client> AvailableClients { get; }

    public ObservableCollection<Reservation> Reservations { get; } = new ObservableCollection<Reservation>();

    public EventCollection Events { get; set; } = new EventCollection();

    public bool IsMacCatalyst { get; } = DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;
    public bool IsPickerSupported => !IsMacCatalyst;

    [ObservableProperty]
    public partial Client? SelectedClientToAdd { get; set; }

    [ObservableProperty]
    public partial string? SeatStatusText { get; set; } = "";

    private int _boatId;

    public int BoatId
    {
        get => _boatId;
        set
        {
            if (SetProperty(ref _boatId, value))
            {
                LoadBoatDataCommand.Execute(value);
            }
        }
    }

    [RelayCommand]
    private Task LoadBoatData(int id)
    {
        BoatTypeUiItem boatType = _boatTypeService.GetBoatTypeById(id);
        CurrentBoatType = boatType;
        Title = boatType.Name;

        InitializeClients();
        UpdateSeatStatus();
        UpdateQualificationFlags();

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task LoadReservationsAsync()
    {
        List<Reservation> reservations = _reservationService.GetAll();
        foreach (Reservation res in reservations)
            ReservationList.Add(res);

        InitializeEvents();

        return Task.CompletedTask;
    }

    public void InitializeEvents()
    {
        foreach (Reservation res in ReservationList)
        {
            DateTime dayOfReservation = res.StartTime;
            if (Events.ContainsKey(dayOfReservation)) continue;

            Events.Add(dayOfReservation, new List<object> { res });
        }
    }

    public Task RefreshReservationListAsync(DateTime value)
    {
        Reservations.Clear();
        foreach (Reservation res in ReservationList)
        {
            if (res.StartTime.Date == value.Date)
            {
                Reservations.Add(res);
            }
        }
        return Task.CompletedTask;
    }

    private void InitializeClients()
    {
        SelectedClients.Clear();
        AvailableClients.Clear();

        Client? currentUser = _clientService.GetCurrentClient();

        if (currentUser != null)
        {
            SelectedClients.Add(currentUser);
        }

        List<Client> allClients = _clientRepository.GetAll();
        foreach (Client client in allClients)
        {
            if (currentUser != null && client.Id == currentUser.Id) continue;

            AvailableClients.Add(client);
        }

        UpdateQualificationFlags();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        _ = ValidateReservationRulesAsync();
        _ = RefreshReservationListAsync(value);
    }
    partial void OnStartTimeChanged(TimeSpan value) => _ = ValidateReservationRulesAsync();
    partial void OnEndTimeChanged(TimeSpan value) => _ = ValidateReservationRulesAsync();

    private async Task ValidateReservationRulesAsync()
    {
        HasDateWarning = false;
        DateWarningText = string.Empty;
        HasTimeWarning = false;
        TimeWarningText = string.Empty;
        HasWeatherWarning = false;

        DateTime startDateTime = SelectedDate.Date + StartTime;
        DateTime endDateTime = SelectedDate.Date + EndTime;

        if (!_reservationService.IsBookingWithinAllowedReservationTime(startDateTime))
        {
            DateWarningText =
                $"$Deze datum is te ver in de toekomst. max {ReservationRules.MaxDaysBeforeReservation} dagen ver";
            HasDateWarning = true;
        }
        else if (_reservationService.IsBookingWithinAllowedReservationTime(startDateTime) && BoatId != 0)
        {
            WeatherAuthorizationResultEnum weatherResult = await _boatAuthorizationService.WeatherAuthorized(BoatId, startDateTime, endDateTime);

            if (weatherResult != WeatherAuthorizationResultEnum.Authorized)
            {
                DateWarningText = MessageHelper.ConvertWeatherAuthorizationMessageToUi(weatherResult);

                HasWeatherWarning = true;
            }

            if (EndTime > StartTime)
            {
                if (!_reservationService.IsValidReservationLength(startDateTime, endDateTime))
                {
                    TimeWarningText = "De reservering duurt te lang. max 2 uur";
                    HasTimeWarning = true;
                }
            }
        }

        SaveReservationCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedClientToAddChanged(Client? value)
    {
        if (value == null) return;
        Client clientToAdd = value;
        SelectedClientToAdd = null;
        AddClientIfValid(clientToAdd);
    }

    private void AddClientIfValid(Client clientToAdd)
    {
        if (clientToAdd == null) return;
        if (CurrentBoatType == null) return;

        if (SelectedClients.Count >= CurrentBoatType.SeatAmount)
        {
            _ = Shell.Current.DisplayAlert("Vol", $"De boot zit vol ({CurrentBoatType.SeatAmount} plaatsen).", "OK");
            return;
        }

        if (SelectedClients.Any(x => x.Id == clientToAdd.Id))
        {
            return;
        }

        SelectedClients.Add(clientToAdd);
        UpdateSeatStatus();
        UpdateQualificationFlags();
    }

    [RelayCommand]
    private void AddClient(Client client)
    {
        AddClientIfValid(client);
    }

    [RelayCommand]
    private void RemoveClient(Client client)
    {
        Client? currentUser = _clientService.GetCurrentClient();
        if (currentUser == null)
        {
            Console.WriteLine("Not logged in");
            return;
        }

        if (client.Id == currentUser?.Id)
        {
            Shell.Current.DisplayAlert("Info", "Je kan jezelf niet verwijderen.", "OK");
            return;
        }

        if (SelectedClients.Contains(client))
        {
            SelectedClients.Remove(client);

            if (!AvailableClients.Any(c => c.Id == client.Id))
            {
                AvailableClients.Add(client);
            }

            UpdateSeatStatus();
            UpdateQualificationFlags();
        }
    }

    private void UpdateSeatStatus()
    {
        if (CurrentBoatType != null)
        {
            string mandatoryText;

            if (SelectedClients.Count != CurrentBoatType.SeatAmount)
            {
                mandatoryText = "Verplicht:";
            }
            else
            {
                mandatoryText = string.Empty;
            }

            SeatStatusText = $"{mandatoryText} {SelectedClients.Count} / {CurrentBoatType.SeatAmount}";
            SaveReservationCommand.NotifyCanExecuteChanged();
        }
    }

    private void UpdateQualificationFlags()
    {
        if (CurrentBoatType == null) return;

        foreach (Client client in SelectedClients)
        {
            if (!_boatAuthorizationService.IsAuthorized(CurrentBoatType.Type, CurrentBoatType.Level, client))
            {
                string levelType = CurrentBoatType.Type == BoatType.S ? "scull" : "sweep";
                int clientLevel = CurrentBoatType.Type == BoatType.S ? client.ScullLevel : client.SweepLevel;
                client.QualificationHelpText =
                    $"Persoon {levelType} level: {clientLevel}. Vereist: {CurrentBoatType.Level}.";
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
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool CanSaveReservation()
    {
        if (HasDateWarning || HasTimeWarning) return false;
        if (CurrentBoatType == null) return false;
        return SelectedClients.Count == CurrentBoatType.SeatAmount;
    }

    private async Task SendReservationEmailAsync()
    {
        string rawBody = await _resourceLoader.LoadEmbeddedResourceAsync("ReservationConfirmation.html");

        if (string.IsNullOrEmpty(rawBody)) return;

        string dateString = SelectedDate.ToString("dd-MM-yyyy");
        string startTimeString = $"{dateString} {StartTime:hh\\:mm}";
        string endTimeString = $"{dateString} {EndTime:hh\\:mm}";

        foreach (Client currentClient in SelectedClients)
        {
            IEnumerable<string> otherRowersList = SelectedClients
                .Where(client => client.Id != currentClient.Id)
                .Select(client => client.FullName);

            string otherRowersText = otherRowersList.Any()
                ? string.Join(", ", otherRowersList)
                : "Geen! Veel plezier solo!";

            string personalizedBody = rawBody
                .Replace("{Name}", currentClient.FullName)
                .Replace("{StartTime}", startTimeString)
                .Replace("{EndTime}", endTimeString)
                .Replace("{OtherRowers}", otherRowersText);

            string subject = $"Reservering #{dateString}";

            List<string> singleReceiver = [currentClient.Email];
            await _mailService.SendMailAsync(singleReceiver, subject, personalizedBody);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveReservation))]
    private async Task SaveReservation()
    {
        DateTime startDateTime = SelectedDate.Date.Add(StartTime);
        DateTime endDateTime = SelectedDate.Date.Add(EndTime);
        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Error", "Eindtijd moet na starttijd zijn.", "OK");
            return;
        }
        if (_reservationService.IsReservationTimeBlocked(Reservations, startDateTime, endDateTime, CurrentBoatType))
        {
            await Shell.Current.DisplayAlert("Error", $"Er zijn geen boten meer beschikbaar van dit type boot op deze tijd", "OK");
            return;
        }
        Reservation currentReservation = new Reservation
        (
            DateTime.Now,
            SelectedDate.Date.Add(StartTime),
            SelectedDate.Date.Add(EndTime),
            _clientService.GetCurrentClient()!.Id,
            BoatId,
            true);

        _reservationService.CreateReservation(currentReservation, SelectedClients.ToList());

        if (!currentReservation.Approved)
        {
            await Shell.Current.DisplayAlert("Info", "Reservering verstuurd naar botencommissaris voor goedkeuring",
                "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Succes", "Reservering Geslaagd!", "OK");
            await SendReservationEmailAsync();
        }

        await Shell.Current.GoToAsync("..");
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
