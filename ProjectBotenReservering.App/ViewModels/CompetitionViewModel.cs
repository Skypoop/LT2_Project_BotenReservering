using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IMatchService _matchService;
    private readonly IMatchRepository _matchRepository;
    private List<Boat> _selectedBoatType;

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

    public CompetitionViewModel(IMatchService matchService, IMatchRepository matchRepository)
    {
        _matchService = matchService;
        _matchRepository = matchRepository;
    }

    [RelayCommand]
    private async Task CreateMatch()
    {
        if (_selectedBoatType  == null)
        {
            return;
        }

        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        List<Reservation> reservations = _matchService.FindOverlappingReservationsForMatch(startDateTime, endDateTime, _selectedBoatType.Select(b => b.Id).ToList());

        if (reservations.Count > 0)
        {
            bool answer = await Shell.Current.DisplayAlert("Attentie reserveringen worden beïnvloed", $"Om ruimte te maken voor deze wedstrijd worden er {reservations.Count} reserveringen geannuleerd. Tijdens het aanmaken, ga je akkoord hiermee?", "OK", "Terug");

            if (answer)
            {
                _matchService.CancelOverlappingReservationsForMatch(reservations);
                //Implement here make the actually match make function
            }
            else
            {
                return;
            }
        } 
        else
        {
            //Implement here make the actually match make function
        }
    }
}