using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class BoatAuthorizationService : IBoatAuthorizationService
{
    private readonly IClientService _clientService;

    public BoatAuthorizationService(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<bool> IsAuthorized(BoatType boatType, int boatLevel)
    {
        Client? client = await _clientService.GetCurrentClient();
        if (client == null) 
        {
            return false;
        }

        return boatType switch
        {
            BoatType.S => client.ScullLevel >= boatLevel,
            BoatType.B => client.RoeiLevel >= boatLevel,
            _=> false
        };
    }

    public async Task<IEnumerable<T>> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector)
    {
        var tasks = items.Select(async item => new
        {
            Item = item,
            Authorized = await IsAuthorized(
            boatTypeSelector(item),
            boatLevelSelector(item))
        });

        var results = await Task.WhenAll(tasks);

        return results.Where(x => x.Authorized).Select(x => x.Item);
    }
}
