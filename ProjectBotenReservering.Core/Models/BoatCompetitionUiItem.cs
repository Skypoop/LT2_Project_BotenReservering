using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectBotenReservering.Core.Models;

public partial class BoatCompetitionUiItem : ObservableObject
{
    public Boat Boat { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsComplete))]
    private string _teamName = string.Empty;

    public ObservableCollection<Client> SelectedClients { get; } = new();

    public bool IsComplete =>
        !string.IsNullOrWhiteSpace(TeamName) &&
        SelectedClients.Count == Capacity;

    public int Capacity => Boat.Seats + (Boat.SteeringWheel ? 1 : 0);

    public BoatCompetitionUiItem(Boat boat)
    {
        Boat = boat;

        SelectedClients.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsComplete));
    }
}
