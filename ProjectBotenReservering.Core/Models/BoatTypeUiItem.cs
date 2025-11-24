namespace ProjectBotenReservering.Core.Models;

public class BoatTypeUiItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int SeatAmount { get; set; }
    public float Weight { get; set; }
    public bool SteeringSeatPresent { get; set; }
    public string? ImagePath { get; set; }
    public int Amount { get; set; }
    public int Level { get; set; }
    public BoatType Type { get; set; }
}