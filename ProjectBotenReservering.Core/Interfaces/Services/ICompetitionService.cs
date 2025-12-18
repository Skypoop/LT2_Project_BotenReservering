namespace ProjectBotenReservering.Core.Interfaces.Services;

using Models;

public interface ICompetitionService
{
    int SelectedBoatId { get; }
    public void ClearCompetitionBoats();
    int AmountBoats { get; set; }
    List<Boat> GetCompetitionBoats();
    (bool IsValid, string? ErrorMessage) ValidateCompetition(DateTime start, DateTime end, List<Boat> boats);
    public Competition? CreateCompetition(DateTime startDate, DateTime endDate, string competitionName);
    bool HasEnoughBoats(int boatId);
    void SetSelectedBoat(int boatId);

    bool IsClientAssignedToAnyTeam(IEnumerable<BoatCompetitionUiItem> items, int clientId);

    public bool IsCompetitionItemComplete(BoatCompetitionUiItem item)
    {
        return !string.IsNullOrWhiteSpace(item.TeamName) &&
               item.SelectedClients.Count == item.Capacity;
    }

    public bool AreAllTeamsComplete(IEnumerable<BoatCompetitionUiItem> items)
    {
        return items.All(item => IsCompetitionItemComplete(item));
    }
    public bool IsClientAlreadyAssigned(IEnumerable<BoatCompetitionUiItem> items, int clientId)
    {
        return items.Any(item => item.SelectedClients.Any(c => c.Id == clientId));
    }

}