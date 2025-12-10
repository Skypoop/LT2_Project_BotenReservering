using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectBotenReservering.App.ViewModels
{
    public partial class CompetitionViewModel : INotifyPropertyChanged
    {
        private readonly IMatchService _matchService;

        private List<Boat> _selectedBoatType =  new List<Boat> { new Boat("Skiff van Kunststof", false, 1, 3, BoatType.S, 45, true, "Local Club") };

        private string _competitionName;
        private DateTime _startDate;
        private TimeSpan _startTime;
        private DateTime _endDate;
        private TimeSpan _endTime;
        private int _teamCount;
        private int _calculatedBoatCount;
        private int _calculatedPersonCount;

        public CompetitionViewModel(IMatchService matchService)
        {
            _matchService = matchService;

            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            _startTime = TimeSpan.Zero;
            _endTime = TimeSpan.Zero;
            _competitionName = string.Empty;
        }

        public string CompetitionName
        {
            get
            {
                return _competitionName;
            }
            set
            {
                if (_competitionName != value)
                {
                    _competitionName = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged();
                }
            }
        }
namespace ProjectBotenReservering.App.ViewModels;

        public int TeamCount
        {
            get
            {
                return _teamCount;
            }
            set
            {
                if (_teamCount != value)
public partial class CompetitionViewModel : BaseViewModel
{
                    _teamCount = value;
                    OnPropertyChanged();
                    RecalculateCounts();
                }
            }
        }
    [ObservableProperty]
    public partial string CompetitionName { get; set; } = string.Empty;

        public int CalculatedBoatCount
        {
            get
            {
                return _calculatedBoatCount;
            }
            private set
            {
                if (_calculatedBoatCount != value)
                {
                    _calculatedBoatCount = value;
                    OnPropertyChanged();
                }
            }
        }
    [ObservableProperty]
    public partial DateTime StartDate { get; set; } = DateTime.Today;

        public int CalculatedPersonCount
        {
            get
            {
                return _calculatedPersonCount;
            }
            private set
            {
                if (_calculatedPersonCount != value)
                {
                    _calculatedPersonCount = value;
                    OnPropertyChanged();
                }
            }
        }
    [ObservableProperty]
    public partial TimeSpan StartTime { get; set; } = TimeSpan.Zero;

        private void RecalculateCounts()
        {
            int boatsNeeded = _teamCount;
            int peopleTotal = _teamCount * 2;
    [ObservableProperty]
    public partial DateTime EndDate { get; set; } = DateTime.Today;

            CalculatedBoatCount = boatsNeeded;
            CalculatedPersonCount = peopleTotal;
        }
    [ObservableProperty]
    public partial TimeSpan EndTime { get; set; } = TimeSpan.Zero;

        public event PropertyChangedEventHandler? PropertyChanged;
    [ObservableProperty]
    public partial int TeamCount { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    [ObservableProperty]
    public partial int CalculatedBoatCount { get; set; }

        [RelayCommand]
        private async Task CreateMatch()
        {
            DateTime startDateTime = StartDate.Date + StartTime;
            DateTime endDateTime = EndDate.Date + EndTime;
    [ObservableProperty]
    public partial int CalculatedPersonCount { get; set; }

            if (_matchService.FindOverlappingReservationForMatch(startDateTime, endDateTime, _selectedBoatType.Select(b => b.Id).ToList()).Count == 0)
    public CompetitionViewModel()
    {
                int amountLappingReservations = _matchService.FindOverlappingReservationForMatch(startDateTime, endDateTime, _selectedBoatType.Select(b => b.Id).ToList()).Count;
                bool answer = await Shell.Current.DisplayAlert("Waarschuwing", $"LET OP: Er zijn {amountLappingReservations}", "Inplannen", "Terug");

                if (answer)
                {
                    //Implement here make the accually match make function
                } else
                {
                    return;
                }
            } else
            {
                //Implement here make the accually match make function
            }
        }
    }
}