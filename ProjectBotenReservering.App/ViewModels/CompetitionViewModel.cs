using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IReservationService _reservationService;
    private readonly ICompetitionService _competitionService;
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
    [ObservableProperty]
    public partial bool SubmitButtonIsEnabeld { get; set; }

    public CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService, IClientService clientService)
    {
        _reservationService = reservationService;
        _competitionService = competitionService;
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, CompetitionBoats.ToList());

        if (!isValid)
        {
            await Shell.Current.DisplayAlert("Fout", errorMessage, "OK");
            return;
        }

        if (await ReservationsNotOverlappingWithTheCompetition(startDateTime, endDateTime))
        {
            SendCompetitionToRepositories();
        }
    }

    private void SendCompetitionToRepositories()
    {
        _competitionService.CreateCompetition(StartDate, EndDate, CompetitionName);
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

        return true;
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
        SubmitButtonIsEnabeld = !string.IsNullOrWhiteSpace(CompetitionName) &&
                                CompetitionBoats.Count > 0;
        // Additional validations for the submit button to be enabled should be added here
    }
}