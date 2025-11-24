using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatAuthorizationService
{
    bool IsAuthorized(BoatType boatType, int boatLevel);
    bool ClientIsAuthorized(BoatType boatType, int boatLevel, Client client);

    IEnumerable<T> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector);
}
