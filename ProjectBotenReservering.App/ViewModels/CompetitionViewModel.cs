using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IReservationService _reservationService;
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

    public CompetitionViewModel(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (!await ValidateCompetitionDateAsync(startDateTime, endDateTime))
        {
            return;
        }

        if (!await ValidateBoatSelectionAsync())
        {
            return;
        }

        if (await ReservationsNotOverlappingWithTheCompetition(startDateTime, endDateTime))
        {
            //Make competition function
        }
    }

    private async Task<bool> ValidateCompetitionDateAsync(DateTime startDateTime, DateTime endDateTime)
    {
        if (!CompetitionValidationHelper.IsCompetitionEndDateValid(startDateTime, endDateTime))
        {
            await Shell.Current.DisplayAlert("Fout", "De eindtijd moet later zijn dan de begintijd.", "OK");
            return false;
        }

        if (!CompetitionValidationHelper.IsCompetitionStartDateValid(startDateTime))
        {
            await Shell.Current.DisplayAlert("Fout", "De begintijd mag niet in het verleden liggen.", "OK");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateBoatSelectionAsync()
    {
        if (!CompetitionValidationHelper.AreBoatsSelected(_selectedBoats))
        {
            await Shell.Current.DisplayAlert("Fout", "Er zijn geen boten geselecteerd.", "OK");
            return false;
        }

        return true;
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