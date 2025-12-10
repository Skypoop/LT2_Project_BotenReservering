using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class CompetitionViewModel : BaseViewModel
{
    private readonly IMatchService _matchService;
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

    public CompetitionViewModel(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [RelayCommand]
    private async Task CreateMatch()
    {
        DateTime startDateTime = StartDate.Date + StartTime;
        DateTime endDateTime = EndDate.Date + EndTime;

        if (_matchService.FindOverlappingReservationForMatch(startDateTime, endDateTime, _selectedBoatType.Select(b => b.Id).ToList()).Count > 0)
        {
            int amountLappingReservations = _matchService.FindOverlappingReservationForMatch(startDateTime, endDateTime, _selectedBoatType.Select(b => b.Id).ToList()).Count;
            bool answer = await Shell.Current.DisplayAlert("Waarschuwing", $"LET OP: Er zijn {amountLappingReservations}", "Inplannen", "Terug");

            if (answer)
            {
                //Implement here make the accually match make function
            }
            else
            {
                return;
            }
        } else
        {
            //Implement here make the accually match make function
        }
    }
}