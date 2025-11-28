using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using Microsoft.Maui.Devices;
using ProjectBotenReservering.App.Context;
using ProjectBotenReservering.Core.Services;
using System.Collections.ObjectModel;
namespace ProjectBotenReservering.App.ViewModels;

[QueryProperty(nameof(BoatId), "Id")]
public partial class ReservationFormViewModel : BaseViewModel
{
    private readonly IBoatTypeService _boatTypeService;
    private readonly IClientService _clientService;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationService _reservationService;
    private readonly IBoatAuthorizationService _boatAuthorizationService;

    public ReservationFormViewModel(
        IBoatTypeService boatTypeService,
        IClientService clientService,
        IClientRepository clientRepository,
        IReservationService reservationService,
        IBoatAuthorizationService boatReservationService,
        ISmtpMailService mailservice
        )
    {
        _mailService = mailservice; 
        _boatTypeService = boatTypeService;
        _clientRepository = clientRepository;

        _clientService = clientService;
        _reservationService = reservationService;
        _boatAuthorizationService = boatReservationService;



        Title = "";

        SelectedDate = DateTime.Today;
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(10, 0, 0);

        SelectedClients = new ObservableCollection<Client>();
        AvailableClients = new ObservableCollection<Client>();
    }

    public DateTime MinDate => DateTime.Today;

    [ObservableProperty] private BoatTypeUiItem? _currentBoatType;

    [ObservableProperty] private DateTime _selectedDate;

    [ObservableProperty] private TimeSpan _startTime;

    [ObservableProperty] private TimeSpan _endTime;

    [ObservableProperty] private string? _dateWarningText;

    [ObservableProperty] private bool _hasDateWarning;

    [ObservableProperty] private string? _timeWarningText;

    [ObservableProperty] private bool _hasTimeWarning;

    private readonly ISmtpMailService _mailService;         
    public ObservableCollection<Client> SelectedClients { get; }
    public ObservableCollection<Client> AvailableClients { get; }

    public bool IsMacCatalyst { get; } = DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;
    public bool IsPickerSupported => !IsMacCatalyst;

    [ObservableProperty] private Client? _selectedClientToAdd;

    [ObservableProperty] private string _seatStatusText = "";

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
    private async Task LoadBoatData(int id)
    {
        BoatTypeUiItem boatType = _boatTypeService.GetBoatTypeById(id);
        CurrentBoatType = boatType;
        Title = boatType.Name;

        InitializeClients();
        UpdateSeatStatus();
        UpdateQualificationFlags();
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
        foreach (var client in allClients)
        {
            if (currentUser != null && client.Id == currentUser.Id) continue;

            AvailableClients.Add(client);
        }

        UpdateQualificationFlags();
    }

    partial void OnSelectedDateChanged(DateTime value) => ValidateReservationRules();
    partial void OnStartTimeChanged(TimeSpan value) => ValidateReservationRules();
    partial void OnEndTimeChanged(TimeSpan value) => ValidateReservationRules();

    private void ValidateReservationRules()
    {
        HasDateWarning = false;
        DateWarningText = string.Empty;
        HasTimeWarning = false;
        TimeWarningText = string.Empty;

        DateTime startDateTime = SelectedDate.Date + StartTime;
        DateTime endDateTime = SelectedDate.Date + EndTime;

        if (!_reservationService.IsBookingWithinAllowedReservationTime(startDateTime))
        {
            //TODO: 2 has to be retrieved from the constant
            DateWarningText = "Deze datum is te ver in de toekomst. max 2 dagen ver";
            HasDateWarning = true;
        }


        if (EndTime > StartTime)
        {
            if (!_reservationService.IsValidReservationLength(startDateTime, endDateTime))
            {
                TimeWarningText = "De reservering duurt te lang. max 2 uur lang";
                HasTimeWarning = true;
            }
        }

        SaveReservationCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedClientToAddChanged(Client value)
    {
        if (value == null) return;
        Client clientToAdd = value;
        SelectedClientToAdd = null;
        TryAddClient(clientToAdd);
    }

    private void TryAddClient(Client clientToAdd)
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
        TryAddClient(client);
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

            // Add back to available list if not there
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
            string verplichtText;

            if (SelectedClients.Count != CurrentBoatType.SeatAmount)
            {
                verplichtText = "Verplicht:";
            }
            else
            {
                verplichtText = string.Empty;
            }

            SeatStatusText = $"{verplichtText} {SelectedClients.Count} / {CurrentBoatType.SeatAmount}";
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
        string rawBody = await ResourceLoaderHelper
            .LoadEmbeddedResourceAsync("ReservationConfirmation.html");

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
        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Error", "Eindtijd moet na starttijd zijn.", "OK");
            return;
        }


        bool anyUnderqualified = SelectedClients.Any(c => c.IsUnderqualified);


        Reservation currentReservation = new Reservation
        (
            DateTime.Now,
            SelectedDate.Date.Add(StartTime),
            SelectedDate.Date.Add(EndTime),
            _clientService.GetCurrentClient()!.Id,
            BoatId, 
            true);
        Console.WriteLine("Reservation created: " + currentReservation);

        if (anyUnderqualified)
        {
            currentReservation.Approved = false;
            _reservationService.Add(currentReservation);
            await Shell.Current.DisplayAlert("Info", "Reservering verstuurd naar botencommissaris voor goedkeuring",
                "OK");
        }
        else
        {
            _reservationService.Add(currentReservation);
            _reservationService.AddClientsToReservation(currentReservation, SelectedClients.ToList ());
            await Shell.Current.DisplayAlert("Succes", "Reservering Geslaagd!", "OK");
            await SendReservationEmailAsync(); 
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private static void ShowQualificationWarning(Client client)
    {
        string message = string.IsNullOrWhiteSpace(client.QualificationHelpText)
            ? "Persoon is te lage rang voor deze boot"
            : client.QualificationHelpText;
        Shell.Current.DisplayAlert("Waarschuwing", message, "OK");
    }
}