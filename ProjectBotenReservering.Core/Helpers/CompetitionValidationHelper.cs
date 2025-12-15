using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Helpers;

public static class CompetitionValidationHelper
{
    public static bool IsCompetitionEndDateValid(DateTime start, DateTime end)
    {
        return end > start;
    }

    public static bool IsCompetitionStartDateValid(DateTime start)
    {
        return start >= DateTime.Now;
    }

    public static bool AreBoatsSelected(List<Boat> boats)
    {
        return boats != null && boats.Count > 0;
    }
}