using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService) : BaseViewModel
{
    private readonly IReservationService _reservationService = reservationService;
    private readonly ICompetitionService _competitionService = competitionService;
    private List<Boat> _selectedBoats;

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
    public partial int TeamCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, _selectedBoats);

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

    private async Task<bool> ReservationsNotOverlappingWithTheCompetition(DateTime startDateTime, DateTime endDateTime)
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
}