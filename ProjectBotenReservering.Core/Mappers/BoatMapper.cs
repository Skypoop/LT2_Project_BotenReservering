using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Mappers;

public static class BoatMapper
{
    public static BoatTypeUiItem BoatToBoatTypeUiItem(Boat boat)
    {
        return new BoatTypeUiItem
        {
            Id = boat.Id,
            Name = boat.Name,
            SeatAmount = boat.Seats,
            Weight = boat.Kg,
            Amount = 1,
            SteeringSeatPresent = boat.SteeringWheel,
            ImagePath = "skiff.jpg",
            Level = boat.Level,
            Type = boat.Type
        };
    }

    public static List<BoatTypeUiItem> BoatsToBoatTypeUiItems(List<Boat> boats)
    {
        List<BoatTypeUiItem> boatTypeUiItems = new List<BoatTypeUiItem>();
        foreach (var boat in boats)
        {
            if (boatTypeUiItems.Any(x => x.Name == boat.Name))
            {
                boatTypeUiItems.First(x => x.Name == boat.Name).Amount++;
            }
            else
            {
                var boatTypeUiItem = BoatToBoatTypeUiItem(boat);
                boatTypeUiItems.Add(boatTypeUiItem);
            }
        }
        
        return boatTypeUiItems;
    }
}