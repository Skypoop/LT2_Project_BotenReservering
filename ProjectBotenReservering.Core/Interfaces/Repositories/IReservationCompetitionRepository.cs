using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories
{
    public interface IReservationCompetitionRepository
    {
        ReservationCompetition Add(ReservationCompetition item);
        ReservationCompetition? Get(int competitionId, int reservationId);
        List<ReservationCompetition> GetByCompetitionId(int competitionId);
        List<ReservationCompetition> GetByReservationId(int reservationId);
        void Delete(int competitionId, int reservationId);
    }
}

