namespace ProjectBotenReservering.Core.Models
{
    public class ReservationMatch
    {
        public int MatchId { get; set; }
        public int ReservationId { get; set; }
        public string TeamName { get; set; }

        public ReservationMatch(int matchId, int reservationId, string teamName)
        {
            MatchId = matchId;
            ReservationId = reservationId;
            TeamName = teamName;
        }
    }
}
