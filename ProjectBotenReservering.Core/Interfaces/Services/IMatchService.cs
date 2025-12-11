using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IMatchService
    {
        void CancelOverlappingReservationsForMatch(List<Reservation> reservations);
        List<Reservation> FindOverlappingReservationsForMatch(DateTime startDate, DateTime endDate, List<int> boatIds);
    }
}
