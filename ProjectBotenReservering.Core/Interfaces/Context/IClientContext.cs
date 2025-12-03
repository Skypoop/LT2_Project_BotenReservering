using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Context
{
    public interface IClientContext
    {
        int GetCurrentClientId();
        void SetCurrentClientId(int id);
    }
}