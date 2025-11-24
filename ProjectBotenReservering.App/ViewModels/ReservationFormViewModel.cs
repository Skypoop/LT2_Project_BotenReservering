using System;
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
        if (value == null) return;

        if (CurrentBoatType == null)
        {
            SelectedClientToAdd = null;
            return;
        }

        if (SelectedClients.Count >= CurrentBoatType.SeatAmount)
        {
            SelectedClientToAdd = null;
            return;
        }

        bool alreadyAdded = false;
        foreach (var x in SelectedClients)
        {
            if (x.Id == value.Id)
            {
                alreadyAdded = true;
                break;
            }
        }

        if (alreadyAdded)
        {
            SelectedClientToAdd = null;
            return;
        }

        SelectedClients.Add(value);

        Client toRemove = null;
        foreach (var c in AvailableClients)
        {
            if (c.Id == value.Id)
            {
                toRemove = c;
                break;
            }
        }

        if (toRemove != null)
            AvailableClients.Remove(toRemove);

        UpdateSeatStatus();
        UpdateQualificationFlags();

        SelectedClientToAdd = null;
    }

    [RelayCommand]
    private void RemoveClient(Client client)
    {
        var currentUser = _clientService.GetCurrentClient();
        if (client.Id == currentUser?.Id)
        {
            return;
        }

        if (SelectedClients.Contains(client))
        {
            SelectedClients.Remove(client);

            bool present = false;
            foreach (var c in AvailableClients)
            {
                if (c.Id == client.Id)
                {
                    present = true;
                    break;
                }
            }

            if (!present)
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
                if (CurrentBoatType.Type == BoatType.S)
                {
                    client.QualificationHelpText = $"Persoon scull level: {client.ScullLevel}. Vereist: {CurrentBoatType.Level}.";
                }
                else
                {
                    client.QualificationHelpText = $"Persoon sweep level: {client.SweepLevel}. Vereist: {CurrentBoatType.Level}.";
                }
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

        bool anyUnderqualified = false;
        foreach (var c in SelectedClients)
        {
            if (c.IsUnderqualified)
            {
                anyUnderqualified = true;
                break;
            }
        }

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