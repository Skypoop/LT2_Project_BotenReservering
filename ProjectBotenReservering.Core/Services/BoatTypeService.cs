using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class BoatTypeService : IBoatTypeService
{
    private readonly IBoatRepository _boatRepository;
    private readonly IBoatAuthorizationService _boatAuthorizationService;
    public BoatTypeService(IBoatRepository boatRepository, IBoatAuthorizationService boatAuthorizationService)
    {
        this._boatRepository = boatRepository;
        this._boatAuthorizationService = boatAuthorizationService;

    }

    public List<BoatTypeUiItem> GetBoatTypes()
    {
        List<Boat> allBoats = _boatRepository.GetAll();
        IEnumerable<Boat> authorizedBoats = _boatAuthorizationService.FilterAuthorized(allBoats, b => b.Type, b => b.Level);
        return BoatMapper.BoatsToBoatTypeUiItems(authorizedBoats.ToList());
    }

    public BoatTypeUiItem GetBoatTypeById(int id)
    {
        Boat? boat = _boatRepository.Get(id);
        if (boat is null)
        {
            throw new KeyNotFoundException($"Boat with id {id} not found");
        }
        return BoatMapper.BoatToBoatTypeUiItem(boat);
    }

    public List<BoatTypeUiItem> FilterBoatTypes(List<BoatTypeUiItem> boatTypeList, bool? hasSteeringWheel, string stringInName, int minWeight)
    {
        List<BoatTypeUiItem> newList = boatTypeList.Where(x => hasSteeringWheel == null || x.SteeringSeatPresent == hasSteeringWheel)
            .Where(x => x.Name.Contains(stringInName, StringComparison.CurrentCultureIgnoreCase))
            .Where(x => x.Weight > minWeight).ToList();
        return newList;
    }
}