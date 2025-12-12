using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories
{
    public interface ICompetitionRepository
    {
        Competition Add(Competition item);
        Competition? Get(int id);
        List<Competition> GetAll();
        void Delete(int id);
        public Competition SaveCompetitionWithReservations(Competition competition, List<int> reservationIds, List<string> teamNames);
    }
}