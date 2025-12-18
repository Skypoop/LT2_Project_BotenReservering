namespace ProjectBotenReservering.Core.Models;

public class Competition 
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? CompetitionName { get; set; }

    public Competition(DateTime startDateTime, DateTime endDateTime, string competitionName, int id = 0)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        CompetitionName = competitionName;
        Id = id;
    }
}