using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class BoatTypeService : IBoatTypeService
{
    private readonly IBoatRepository boatRepository;
    public BoatTypeService(IBoatRepository boatRepository)
    {
        this.boatRepository = boatRepository;
    }

    public List<BoatTypeUiItem> GetBoatTypes()
    {
        return BoatMapper.BoatsToBoatTypeUiItems(boatRepository.GetAll());
    }
    
    public List<BoatTypeUiItem> FilterBoatTypes(List<BoatTypeUiItem> boatTypeList, bool hasSteeringWheel, string hasStringInName, int hasMinWeight)
    {
        List<BoatTypeUiItem> newList = boatTypeList.Where(x => x.SteeringSeatPresent == hasSteeringWheel)
            .Where(x => x.Name.Contains(hasStringInName, StringComparison.CurrentCultureIgnoreCase))
            .Where(x => x.Weight > hasMinWeight).ToList();
        return newList;
    }
}