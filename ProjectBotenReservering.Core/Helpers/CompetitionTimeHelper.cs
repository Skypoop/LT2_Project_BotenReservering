namespace ProjectBotenReservering.Core.Helpers;

public static class CompetitionTimeHelper
{
    public static DateTime CombineDateAndTime(DateTime date, TimeSpan time)
    {
        return date.Date + time;
    }

    public static DateTime GetStartTimeWithPreparation(DateTime date, TimeSpan time)
    {
        return CombineDateAndTime(date, time).AddMinutes(-30);
    }
}