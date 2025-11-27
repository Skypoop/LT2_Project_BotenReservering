using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IBoatService
    {
        Boat Get(int id);
    }
}
