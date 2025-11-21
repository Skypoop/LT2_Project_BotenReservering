namespace ProjectBotenReservering.Core.Models;

public class WindConstraint
{
    public int Windforce { get; set; }
    public int MinScullLevel { get; set; }
    public int MinRoeiLevel { get; set; }

    public WindConstraint(int windforce, int minScullLevel, int minRoeiLevel)
    {
        Windforce = windforce;
        MinScullLevel = minScullLevel;
        MinRoeiLevel = minRoeiLevel;
    }
}

