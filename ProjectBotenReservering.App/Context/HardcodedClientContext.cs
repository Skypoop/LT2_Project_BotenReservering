namespace ProjectBotenReservering.App.Context;
using ProjectBotenReservering.Core.Interfaces.Context;

public class HardcodedClientContext : IClientContext
{
   public int GetCurrentClientId() => 5; // Placeholder until proper authentication is implemented
}// Note: Id 3 has maximum levels