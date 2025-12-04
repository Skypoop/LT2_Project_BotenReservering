using ProjectBotenReservering.Core.Interfaces.Context;

namespace ProjectBotenReservering.Core.Context
{
    public class ClientContext : IClientContext
    {
        private int _currentClientId;

        public int GetCurrentClientId()
        {
            return _currentClientId;
        }

        public void SetCurrentClientId(int id)
        {
            _currentClientId = id;
        }
    }
}