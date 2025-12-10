using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IMatchService
    {
        void DeleteOverlappingReservationForMatch(int matchId, List<Reservation> reservations);
        List<Reservation> FindOverlappingReservationForMatch(DateTime startDate, DateTime endDate, List<int> boatIds);
    }
}
