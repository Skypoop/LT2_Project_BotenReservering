using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatTypeService
{
    public List<BoatTypeUiItem> GetBoatTypes();
    public List<BoatTypeUiItem> FilterBoatTypes(List<BoatTypeUiItem> boatTypeList, bool? hasSteeringWheel, string hasStringInName, int hasMinWeight);
    public BoatTypeUiItem GetBoatTypeById(int id);
}