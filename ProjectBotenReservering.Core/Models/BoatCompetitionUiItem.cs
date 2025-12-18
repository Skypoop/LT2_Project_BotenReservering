using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectBotenReservering.Core.Models;

public class BoatCompetitionUiItem : INotifyPropertyChanged
{
    public Boat Boat { get; }

    private string _teamName = string.Empty;
    public string TeamName
    {
        get => _teamName;
        set
        {
            if (_teamName != value)
            {
                _teamName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsComplete));
            }
        }
    }

    public ObservableCollection<Client> SelectedClients { get; } = new();

    public bool IsComplete =>
        !string.IsNullOrWhiteSpace(TeamName) &&
        SelectedClients.Count == Capacity &&
        Capacity > 0;

    public int Capacity => Boat.Seats + (Boat.SteeringWheel ? 1 : 0);

    public BoatCompetitionUiItem(Boat boat)
    {
        Boat = boat;

        SelectedClients.CollectionChanged += (s, e) => {
            OnPropertyChanged(nameof(IsComplete));
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}