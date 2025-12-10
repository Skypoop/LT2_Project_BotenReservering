using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services
{
    public class MatchService : IMatchService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMatchRepository _matchRepository;

        public MatchService(IReservationRepository reservationRepository, IMatchRepository matchRepository)
        {
            _reservationRepository = reservationRepository;
            _matchRepository = matchRepository;
        }

        public void DeleteOverlappingReservationForMatch(int matchId, List<Reservation> reservations)
        {
            foreach (Reservation reservation in reservations)
            {
                _matchRepository.CancelReservationAndUpdateStatus(reservation.Id, matchId);
            }
        }

        public List<Reservation> FindOverlappingReservationForMatch(DateTime startDate, DateTime endDate, List<int> boatIds)
        {
            List<Reservation> reservations = _reservationRepository.GetAll();

            return reservations.Where(r => r.StartTime < endDate && r.EndTime > startDate && boatIds.Contains(r.BoatId)).ToList();
        }
    }
}
