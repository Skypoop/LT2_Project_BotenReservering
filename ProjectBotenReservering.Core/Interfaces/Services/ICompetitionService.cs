namespace ProjectBotenReservering.Core.Interfaces.Services;

using Models;

public interface ICompetitionService
{
    int SelectedBoatId { get; set; }
    int AmountBoats { get; set; }

    List<Boat> GetCompetitionBoats();
}