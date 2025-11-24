using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IBoatTypeService
{
    public Task<List<BoatTypeUiItem>> GetBoatTypes();
    public Task<List<BoatTypeUiItem>> FilterBoatTypes(List<BoatTypeUiItem> boatTypeList, bool? hasSteeringWheel, string hasStringInName, int hasMinWeight);
}