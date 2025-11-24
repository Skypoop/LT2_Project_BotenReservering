namespace ProjectBotenReservering.Core.Models;

public class WindConstraint
{
    public int Windforce { get; set; }
    public int MinScullLevel { get; set; }
    public int MinSweepLevel { get; set; }

    public WindConstraint(int windforce, int minScullLevel, int minSweepLevel)
    {
        Windforce = windforce;
        MinScullLevel = minScullLevel;
        MinSweepLevel = minSweepLevel;
    }
}

