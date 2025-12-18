using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class BoatAuthorizationService : IBoatAuthorizationService
{
    private readonly IClientService _clientService;
    private readonly IWeatherService _weatherService;
    private readonly IBoatRepository _boatRepository;
    private readonly IWindConstraintRepository _windConstraintRepository;

    public BoatAuthorizationService(IClientService clientService, IWeatherService weatherService, IBoatRepository boatRepository, IWindConstraintRepository windConstraintRepository)
    {
        _clientService = clientService;
        _weatherService = weatherService;
        _boatRepository = boatRepository;
        _windConstraintRepository = windConstraintRepository;
    }

    private bool AuthorizationCheck(BoatType boatType, int boatLevel, Client? client)
    {
        if (client == null)
        {
            return false;
        }

        return boatType switch
        {
            BoatType.S => client.ScullLevel >= boatLevel,
            BoatType.B => client.SweepLevel >= boatLevel,
            _ => false
        };
    }
    /// <summary>
    /// This method checks for the weather conditions for the boat(s)
    /// </summary>
    /// <returns>
    /// 0(int) = Authorized <br/>
    /// 1(int) = Unauthorized (boat requires a higher level) <br/>
    /// 2(int) = Unauthorized (date too far in the future) <br/>
    /// </returns>
    public async Task<int> WeatherAuthorized(int boatId, DateTime beginDate, DateTime endDate)
    {
        if(beginDate.Subtract(DateTime.Now).TotalDays > 7)
        {
            return 2;
        }

        if(endDate.Subtract(DateTime.Now).TotalDays > 7)
        {
            return 2;
        }

        int windforce = await _weatherService.GetWeatherAsync(beginDate, endDate);
        WindConstraint? minLevels = _windConstraintRepository.Get(windforce);
        Boat? boat = _boatRepository.Get(boatId);

        if (minLevels == null || boat == null)
        {
            return 1;
        }

        if ((boat.Type == BoatType.B && boat.Level <= minLevels.MinSweepLevel) || (boat.Type == BoatType.S && boat.Level <= minLevels.MinScullLevel))
        {
            return 1;
        }

        return 0;
    }

    public bool IsAuthorized(BoatType boatType, int boatLevel) => IsAuthorized(boatType, boatLevel, _clientService.GetCurrentClient());

    public bool IsAuthorized(BoatType boatType, int boatLevel, Client? client)
    {
        return AuthorizationCheck(boatType, boatLevel, client);
    }

    public bool IsAuthorized(int boatId, Client client)
    {
        Boat? boat = _boatRepository.Get(boatId);
        if (boat == null) return false;
        return IsAuthorized(boat.Type, boat.Level, client);
    }

    public IEnumerable<T> FilterAuthorized<T>(IEnumerable<T> items, Func<T, BoatType> boatTypeSelector, Func<T, int> boatLevelSelector)
    {
        return items.Where(item => IsAuthorized(boatTypeSelector(item), boatLevelSelector(item)));
    }
}
