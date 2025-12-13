namespace ProjectBotenReservering.Core.Models
{
    public class ReservationCompetition
    {
        public int CompetitionId { get; set; }
        public int ReservationId { get; set; }
        public string TeamName { get; set; }

        public ReservationCompetition(int competitionId, int reservationId, string teamName)
        {
            CompetitionId = competitionId;
            ReservationId = reservationId;
            TeamName = teamName;
        }
    }
}
