namespace ProjectBotenReservering.Core.Models;

public class Boat
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool SteeringWheel { get; set; }
    public int Seats { get; set; }
    public int Level { get; set; }
    public char Type { get; set; }
    public int Kg { get; set; }
    public bool Operational { get; set; }
    public string? Club { get; set; }

    public Boat(int id, string name, bool steeringWheel, int seats, int level, char type, int kg, bool operational, string? club)
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

