using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public ReservationFormViewModel(
        IBoatTypeService boatTypeService,
        IClientService clientService,
        IClientRepository clientRepository)
    {
        _boatTypeService = boatTypeService;
        _clientService = clientService;
        _clientRepository = clientRepository;

        Title = "";

        SelectedDate = DateTime.Today;
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(10, 0, 0);

        SelectedClients = new ObservableCollection<Client>();
        AvailableClients = new ObservableCollection<Client>();
    }

    public DateTime MinDate => DateTime.Today;

    [ObservableProperty]
    private BoatTypeUiItem _currentBoatType;

    [ObservableProperty]
    private DateTime _selectedDate;

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    public ObservableCollection<Client> SelectedClients { get; }
    public ObservableCollection<Client> AvailableClients { get; }

    [ObservableProperty]
    private Client _selectedClientToAdd;

    [ObservableProperty]
    private string _seatStatusText = "";

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
        var boatType = _boatTypeService.GetBoatTypeById(id);
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

        var currentUser = _clientService.GetCurrentClient();

        if (currentUser != null)
        {
            SelectedClients.Add(currentUser);
        }

        var allClients = _clientRepository.GetAll();
        foreach (var client in allClients)
        {
            if (currentUser != null && client.Id == currentUser.Id) continue;

            AvailableClients.Add(client);
        }

        UpdateQualificationFlags();
    }

    partial void OnSelectedClientToAddChanged(Client value)
    {
        // 1. If null, do nothing (this happens when we reset the picker)
        if (value == null) return;

        // 2. Capture the client to add
        var clientToAdd = value;

        // 3. IMMEDIATELY reset the picker to null. 
        // This stops the "Double Add" bug by clearing the selection before we mess with the list.
        SelectedClientToAdd = null;

        if (CurrentBoatType == null) return;

        // 4. Check Capacity
        if (SelectedClients.Count >= CurrentBoatType.SeatAmount)
        {
            Shell.Current.DisplayAlert("Vol", $"De boot zit vol ({CurrentBoatType.SeatAmount} plaatsen).", "OK");
            return;
        }

        // 5. Check Duplicates
        if (SelectedClients.Any(x => x.Id == clientToAdd.Id))
        {
            return;
        }

        // 6. Add to Selected List
        SelectedClients.Add(clientToAdd);
        UpdateSeatStatus();
        UpdateQualificationFlags();

        // 7. Remove from Available List with a DELAY.
        // We must wait for the UI to finish processing "SelectedClientToAdd = null".
        // If we remove it too fast, the Picker grabs the next person in the list automatically.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100); // Small delay to let the UI settle

            var itemToRemove = AvailableClients.FirstOrDefault(c => c.Id == clientToAdd.Id);
            if (itemToRemove != null)
            {
                AvailableClients.Remove(itemToRemove);
            }
        });
    }

    [RelayCommand]
    private void RemoveClient(Client client)
    {
        var currentUser = _clientService.GetCurrentClient();
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
            SeatStatusText = $"{SelectedClients.Count} / {CurrentBoatType.SeatAmount}";
            SaveReservationCommand.NotifyCanExecuteChanged();
        }
    }

    private void UpdateQualificationFlags()
    {
        if (CurrentBoatType == null) return;

        foreach (var client in SelectedClients)
        {
            bool underqualified = CurrentBoatType.Type switch
            {
                BoatType.S => client.ScullLevel < CurrentBoatType.Level,
                BoatType.B => client.SweepLevel < CurrentBoatType.Level,
                _ => false
            };

            client.IsUnderqualified = underqualified;

            if (underqualified)
            {
                string levelType = CurrentBoatType.Type == BoatType.S ? "scull" : "sweep";
                int clientLevel = CurrentBoatType.Type == BoatType.S ? client.ScullLevel : client.SweepLevel;
                client.QualificationHelpText = $"Persoon {levelType} level: {clientLevel}. Vereist: {CurrentBoatType.Level}.";
            }
            else
            {
                client.QualificationHelpText = string.Empty;
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
        if (CurrentBoatType == null) return false;
        return SelectedClients.Count == CurrentBoatType.SeatAmount;
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

        if (anyUnderqualified)
        {
            await Shell.Current.DisplayAlert("Info", "Reservering verstuurd naar botencommissaris voor goedkeuring", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Succes", "Reservering Geslaagd!", "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void ShowQualificationWarning(Client client)
    {
        var message = string.IsNullOrWhiteSpace(client.QualificationHelpText) ? "Persoon is te lage rang voor deze boot" : client.QualificationHelpText;
        Shell.Current.DisplayAlert("Waarschuwing", message, "OK");
    }
}