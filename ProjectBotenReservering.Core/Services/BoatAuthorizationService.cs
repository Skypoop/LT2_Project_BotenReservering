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

    public bool IsAuthorized(BoatType boatType, int boatLevel)
    {
        Client? client = _clientService.GetCurrentClient();
        if (client == null) 
        {
            return false;
        }

        return boatType switch
        {
            BoatType.S => client.ScullLevel >= boatLevel,
            BoatType.B => client.SweepLevel >= boatLevel,
            _=> false
        };
    }

    public IEnumerable<T> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector)
    {
        return items.Where(item => IsAuthorized(boatTypeSelector(item), boatLevelSelector(item)));
    }
}
