using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectBotenReservering.App.ViewModels
{
    public class CompetitionViewModel : INotifyPropertyChanged
    {
        private string _competitionName;
        private DateTime _startDate;
        private TimeSpan _startTime;
        private DateTime _endDate;
        private TimeSpan _endTime;
        private int _teamCount;
        private int _calculatedBoatCount;
        private int _calculatedPersonCount;

        public CompetitionViewModel()
        {
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

        public int TeamCount
        {
            get
            {
                return _teamCount;
            }
            set
            {
                if (_teamCount != value)
                {
                    _teamCount = value;
                    OnPropertyChanged();
                    RecalculateCounts();
                }
            }
        }

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

        private void RecalculateCounts()
        {
            int boatsNeeded = _teamCount;
            int peopleTotal = _teamCount * 2;

            CalculatedBoatCount = boatsNeeded;
            CalculatedPersonCount = peopleTotal;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}