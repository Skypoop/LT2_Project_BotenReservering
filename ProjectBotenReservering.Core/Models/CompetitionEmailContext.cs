using System.Collections.ObjectModel;
namespace ProjectBotenReservering.Core.Models;
public record CompetitionEmailContext(
string CompetitionName,
DateTime StartTimeWithPreparation,
DateTime EndDateTime,
Dictionary<int, ObservableCollection<Client>> ClientsByBoatId,
Dictionary<int, string> TeamNameByBoatId,
List<Boat> CompetitionBoats
);
