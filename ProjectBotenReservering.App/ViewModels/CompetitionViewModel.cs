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

    private List<Boat> _selectedBoats;
    private string _teamCount = "0";

    public string TeamCount
    {
        get => _teamCount;
        set
        {
            if (SetProperty(ref _teamCount, value))
            {
                if (CheckBoatAmounIsValid(value))
                {
                    SelectMatchBoatTypeIsEnable = true;
                    _competitionService.AmountBoats = int.Parse(value);
                }
                else
                {
                    SelectMatchBoatTypeIsEnable = false;
                }
            }
        }
    }

    [ObservableProperty]
    private ObservableCollection<Boat> competitionBoats = new ObservableCollection<Boat>();

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
    public partial bool SelectMatchBoatTypeIsEnable { get; set; } = false;

    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    public CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService)
    {
        _reservationService = reservationService;
        _competitionService = competitionService;
    }

    [RelayCommand]
    private async Task CreateMatch()
    {
        if (_selectedBoats == null)
        {
            return;
        }

        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (await ReservationsNotOverlappingWithTheMatch(startDateTime, endDateTime))
        {
            //Make match function
        } 
    }

    [RelayCommand]
    private async Task SelectMatchBoatType()
    {
        await Shell.Current.GoToAsync(nameof(BoatTypeSelectionMatchView));
    }


    private async Task<bool> ReservationsNotOverlappingWithTheMatch(DateTime startDateTime, DateTime endDateTime)
    {
        List<Reservation> overlappingReservations = _reservationService.FindOverlappingReservations(startDateTime, endDateTime, _selectedBoats.Select(b => b.Id).ToList());

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

    private bool CheckBoatAmounIsValid(string boatAmount)
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
}