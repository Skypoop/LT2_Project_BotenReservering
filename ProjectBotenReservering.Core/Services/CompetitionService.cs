using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionService : ICompetitionService
{
    public CompetitionService()
    {
    }

    public (bool IsValid, string? ErrorMessage) ValidateCompetition(DateTime start, DateTime end, List<Boat> boats)
    {
        if (!CompetitionValidationHelper.IsCompetitionEndDateValid(start, end))
        {
            return (false, "De einddatum moet later zijn dan de begindatum.");
        }

        if (!CompetitionValidationHelper.IsCompetitionStartDateValid(start))
        {
            return (false, "De begindatum mag niet in het verleden liggen.");
        }

        if (!CompetitionValidationHelper.AreBoatsSelected(boats))
        {
            return (false, "Er zijn geen boten geselecteerd.");
        }

        return (true, null);
    }
}