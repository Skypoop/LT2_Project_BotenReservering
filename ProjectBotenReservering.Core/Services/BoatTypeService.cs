using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class BoatTypeService : IBoatTypeService
{
    public List<BoatType> FilterBoatTypes(List<BoatType> boatTypeList, bool hasSteeringWheel, string hasStringInName, int hasMinWeight)
    {
        List<BoatType> newList = boatTypeList.Where(x => x.SteeringSeatPresent == hasSteeringWheel)
            .Where(x => x.Name.Contains(hasStringInName, StringComparison.CurrentCultureIgnoreCase))
            .Where(x => x.Weight > hasMinWeight).ToList();
        return newList;
    }
}