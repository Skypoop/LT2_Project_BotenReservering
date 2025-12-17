using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel(IReservationService reservationService, ICompetitionService competitionService) : BaseViewModel
{
    private readonly IReservationService _reservationService = reservationService;
    private readonly ICompetitionService _competitionService = competitionService;

    [ObservableProperty]
    public partial string TeamCount { get; set; } = "0";

    partial void OnTeamCountChanged(string value)
    {
        if (int.TryParse(value, out int teamCount))
        {
            SelectCompetitionBoatTypeIsEnable = teamCount > 1;

            _competitionService.AmountBoats = int.Parse(value);
        }
        else
        {
            SelectCompetitionBoatTypeIsEnable = false;
        }
    }

    [ObservableProperty]
    public partial ObservableCollection<Boat> CompetitionBoats { get; set; } = new ObservableCollection<Boat>();

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

    [RelayCommand]
    private async Task CreateCompetition()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;
        
        if (await CompetitionIsValid() == false)
            return;
        
        if (await HandleWarningPopup() && await HandleConflictingReservationsAsync(startDateTime, endDateTime))
        {
            await SaveCompetition();
            RefreshScreen();
            
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

    private void RefreshScreen()
    {
        CompetitionName = String.Empty;
        StartDate = DateTime.Today;
        StartTime = TimeSpan.Zero;
        EndDate = DateTime.Today;
        EndTime = TimeSpan.Zero;
        TeamCount = "0";
        _competitionService.AmountBoats = 0;
        _competitionService.ClearCompetitionBoats();
        CompetitionBoats.Clear();
        RefreshCompetitionCounters();
    }
    
    private async Task<bool> HandleWarningPopup()
    {
        string message = CreateConfirmationPopupMessage();
        return await Shell.Current.DisplayAlert("Bevestigen", message, "Bevestigen", "Terug");
    }

    private string CreateConfirmationPopupMessage()
    {
        string message = "Waarschuwingen:\n";
        // Add warnings to message here
        
        // ---
        
        message += "\n- De wedstrijd zal worden aangemaakt met de opgegeven gegevens.";
        
        return message;
    }
    
    private async Task<bool> CompetitionIsValid()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;


        (bool isValid, string? errorMessage) = _competitionService.ValidateCompetition(startDateTime, endDateTime, CompetitionBoats.ToList());

        if (!isValid)
            await Shell.Current.DisplayAlert("Fout", errorMessage, "OK");
        
        return isValid;
    }
    
    private async Task SaveCompetition()
    {
        _competitionService.CreateCompetition(StartDate + StartTime, EndDate + EndTime, CompetitionName);
        await ShowCompleteionMessage();
        return;
    }

    private async Task ShowCompleteionMessage()
    {
        await Shell.Current.DisplayAlert("Wedstrijd Aangemaakt", "De wedstrijd is succesvol aangemaakt.", "OK");
        return;
    }
    
    [RelayCommand]
    private async Task SelectCompetitionBoatType()
    {
        await Shell.Current.GoToAsync(nameof(BoatTypeSelectionCompetitionView));
    }

    private async Task<bool> HandleConflictingReservationsAsync(DateTime startDateTime, DateTime endDateTime)
    {
        List<int> boatIds = [.. (CompetitionBoats ?? Enumerable.Empty<Boat>()).Select(boat => boat.Id)];
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

    private static async Task<bool> ConfirmCancellationWithUserAsync(int count)
    {
        return await Shell.Current.DisplayAlert("Attentie reserveringen worden beïnvloed", $"Om ruimte te maken voor deze wedstrijd worden er {count} reserveringen geannuleerd. Tijdens het aanmaken, ga je akkoord hiermee?", "OK", "Terug");
    }

    private void CancelReservations(List<Reservation> overlappingReservations)
    {
        _reservationService.CancelOverlappingReservations(overlappingReservations);
    }

    public void FillBoatCompetitionsList()
    {
        CompetitionBoats.Clear();

        List<Boat> boats = _competitionService.GetCompetitionBoats();

        foreach (Boat boat in boats)
        {
            CompetitionBoats.Add(boat);
        }

        RefreshCompetitionCounters();
    }

    public void RefreshCompetitionCounters()
    {
        CalculatedBoatCount = CompetitionBoats.Count;
        
        Boat? boat = CompetitionBoats.FirstOrDefault();
        CalculatedPersonCount = boat?.Seats * CompetitionBoats.Count ?? 0;
        
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