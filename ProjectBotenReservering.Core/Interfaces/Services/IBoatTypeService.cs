using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatTypeService
{
    public List<BoatType> FilterBoatTypes(List<BoatType> boatTypeList, bool hasSteeringWheel, string hasStringInName, int hasMinWeight);
}