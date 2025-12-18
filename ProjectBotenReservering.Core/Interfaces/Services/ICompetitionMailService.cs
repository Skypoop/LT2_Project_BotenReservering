using System.Collections.ObjectModel;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public record CompetitionEmailContext(
    string CompetitionName,
    DateTime StartTimeWithPreparation,
    DateTime EndDateTime,
    IReadOnlyDictionary<int, ObservableCollection<Client>> ClientsByBoatId,
    IReadOnlyDictionary<int, string> TeamNameByBoatId,
    IReadOnlyCollection<Boat> CompetitionBoats
);

public interface ICompetitionMailService
{
    Task SendCompetitionConfirmationEmailsAsync(CompetitionEmailContext context);
}