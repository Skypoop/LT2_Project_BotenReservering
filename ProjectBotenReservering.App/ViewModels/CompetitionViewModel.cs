using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IReservationService _reservationService;
    private List<Boat>? _selectedBoats;

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
    private async Task CreateMatch()
    {
        // Hasn't been implemented yet. Comment out if you want to test navigation to tweet creation.
        //if (_selectedBoats == null)
        //{
        //    return;
        //}

        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (await HandleConflictingReservationsAsync(startDateTime, endDateTime))
        {
            // Construct context string from user input for the tweet
            string contextString = $"Naam: {CompetitionName}, " +
                                   $"Datum: {StartDate:dd-MM-yyyy}, " +
                                   $"Tijd: {StartTime:hh\\:mm} - {EndTime:hh\\:mm}, " +
                                   $"Aantal Teams: {TeamCount}";
            // TODO: Add team names to context when implemented in UI
            // Navigate to TweetCreationView and pass the context
            Dictionary<string, object> navigationParameter = new()
            {
                { "context", contextString }
            };
            // May have to be moved to popup as discussed in wireframe design
            await Shell.Current.GoToAsync(nameof(TweetCreationView), navigationParameter);
        }
    }

    private async Task<bool> HandleConflictingReservationsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        List<int> boatIds = (_selectedBoats ?? Enumerable.Empty<Boat>()).Select(boat => boat.Id).ToList();
        List<Reservation> overlappingReservations = _reservationService.FindOverlappingReservations(startDateTime, endDateTime, boatIds);

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

    private async Task<bool> ConfirmCancellationWithUserAsync(int count)
    {
        return await Shell.Current.DisplayAlert("Attentie reserveringen worden beïnvloed", $"Om ruimte te maken voor deze wedstrijd worden er {count} reserveringen geannuleerd. Tijdens het aanmaken, ga je akkoord hiermee?", "OK", "Terug");
    }

    private void CancelReservations(List<Reservation> overlappingReservations)
    {
        _reservationService.CancelOverlappingReservations(overlappingReservations);
    }
}