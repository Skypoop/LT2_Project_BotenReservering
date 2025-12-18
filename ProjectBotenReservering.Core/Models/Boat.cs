namespace ProjectBotenReservering.Core.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

public enum BoatType
{
    B, // Sweeping boat
    S  // Sculling boat
}

public partial class Boat : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool SteeringWheel { get; set; }
    public int Seats { get; set; }
    public int Level { get; set; }
    public BoatType Type { get; set; }
    public int Kg { get; set; }
    public bool Operational { get; set; }
    public string? Club { get; set; }

    [NotMapped]
    [ObservableProperty]
    private bool _isComplete;

    public Boat(string name, bool steeringWheel, int seats, int level, BoatType type, int kg, bool operational, string? club, int id = 0)
    {
        Id = id;
        Name = name;
        SteeringWheel = steeringWheel;
        Seats = seats;
        Level = level;
        Type = type;
        Kg = kg;
        Operational = operational;
        Club = club;
    }
}

