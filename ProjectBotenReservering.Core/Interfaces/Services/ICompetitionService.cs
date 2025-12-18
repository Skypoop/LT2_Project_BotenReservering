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

}