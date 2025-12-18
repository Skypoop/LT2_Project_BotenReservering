using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatAuthorizationService
{
    bool IsAuthorized(BoatType boatType, int boatLevel);
    bool IsAuthorized(BoatType boatType, int boatLevel, Client client);
    bool IsAuthorized(int boatId, Client client);

    Task<WeatherAuthorizationResultEnum> WeatherAuthorized(int boatId, DateTime beginDate, DateTime endDate);

    IEnumerable<T> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector);
}
