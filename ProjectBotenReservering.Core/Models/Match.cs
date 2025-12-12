namespace ProjectBotenReservering.Core.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? MatchName { get; set; }
    }
}

