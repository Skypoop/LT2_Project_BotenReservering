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
            SeatAmmount = boat.Seats,
            Weight = boat.Kg,
            Ammount = 1,
            SteeringSeatPresent = boat.SteeringWheel,
            ImagePath = "skiff.jpg" 
        };
    }

    public static List<BoatTypeUiItem> BoatsToBoatTypeUiItems(List<Boat> boats)
    {
        List<BoatTypeUiItem> boatTypeUiItems = new List<BoatTypeUiItem>();
        foreach (var boat in boats)
        {
            if (boatTypeUiItems.Any(x => x.Name == boat.Name))
            {
                boatTypeUiItems.First(x => x.Name == boat.Name).Ammount++;
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