using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories
{
    public interface IReservationMatchRepository
    {
        ReservationMatch Add(ReservationMatch item);
        ReservationMatch? Get(int matchId, int reservationId);
        List<ReservationMatch> GetByMatchId(int matchId);
        List<ReservationMatch> GetByReservationId(int reservationId);
        void Delete(int matchId, int reservationId);
    }
}

