namespace ProjectBotenReservering.Core.Interfaces.Services;

using Models;

public interface ICompetitionService
{
    int SelectedBoatId { get; }
    public void ClearCompetitionBoats();
    int AmountBoats { get; set; }
    List<Boat> GetCompetitionBoats();
    (bool IsValid, string? ErrorMessage) ValidateCompetition(DateTime start, DateTime end, List<Boat> boats);

    public Competition? CreateCompetition(DateTime startDate, DateTime endDate, string competitionName, List<BoatCompetitionUiItem> competitionUiItems);
    bool HasEnoughBoats(int boatId);
    public bool SetSelectedBoat(int boatId, DateTime startTime, DateTime endTime);

    bool IsClientAssignedToAnyTeam(IEnumerable<BoatCompetitionUiItem> items, int clientId);
    bool IsCompetitionItemComplete(BoatCompetitionUiItem item);
    bool AreAllTeamsComplete(IEnumerable<BoatCompetitionUiItem> items);

}