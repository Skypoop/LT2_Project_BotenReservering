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
    
    public List<BoatTypeUiItem> FilterBoatTypes(List<BoatTypeUiItem> boatTypeList, bool hasSteeringWheel, string stringInName, int minWeight)
    {
        List<BoatTypeUiItem> newList = boatTypeList.Where(x => x.SteeringSeatPresent == hasSteeringWheel)
            .Where(x => x.Name.Contains(stringInName, StringComparison.CurrentCultureIgnoreCase))
            .Where(x => x.Weight > minWeight).ToList();
        return newList;
    }
}