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
    public partial bool SelectCompetitionBoatTypeIsEnable { get; set; } = false;

    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    public ObservableCollection<Client> SelectedClients { get; }
    public ObservableCollection<Client> AvailableClients { get; }

    [ObservableProperty]
    public partial Client? SelectedClient { get; set; }

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

        List<Boat> boats = _competitionService.GetCompetitionBoats();

        foreach (Boat boat in boats)
        {
            CompetitionBoats.Add(boat);
        }

        RefreshCompetitionCounters(boats);
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
    }
    partial void OnSelectedClientChanged(Client? value)
    {
        if (value == null) return;
        Client clientToAdd = value;
        SelectedClient = null;

        // client toeveogen aan lijst van boatId
        AddClientIfValid(clientToAdd);
    }

    partial void OnSelectedBoatChanged(Boat? value)
    {
        
    }

    private void AddClientIfValid(Client clientToAdd)
    {
        List<Boat> listboats = _competitionService.GetCompetitionBoats();
        Boat singleBoat = listboats.FirstOrDefault();
        
        if (clientToAdd == null) return;
        if (singleBoat.Type == null) return;

        if (SelectedClients.Count >= singleBoat.Seats)
        {
            _ = Shell.Current.DisplayAlert("Vol", $"De boot zit vol ({singleBoat.Seats} plaatsen).", "OK");
            return;
        }

        if (SelectedClients.Any(x => x.Id == clientToAdd.Id))
        {
            return;
        }

        SelectedClients.Add(clientToAdd);
        //UpdateSeatStatus();
        //UpdateQualificationFlags();
    }
}