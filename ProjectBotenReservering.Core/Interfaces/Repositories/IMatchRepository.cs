using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories
{
    public interface IMatchRepository
    {
        Match Add(Match item);
        Match? Get(int id);
        List<Match> GetAll();
        void Delete(int id);
        Match SaveMatchWithReservations(Match match, List<int> reservationIds, List<string> teamNames);
        void CancelReservationAndUpdateStatus(int reservationId, int matchId);
    }
}
