using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatAuthorizationService
{
    Task<bool> IsAuthorized(BoatType boatType, int boatLevel);
    Task<IEnumerable<T>> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector);
}
