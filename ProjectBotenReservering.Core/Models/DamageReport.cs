namespace ProjectBotenReservering.Core.Models;

public class DamageReport
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int BoatId { get; set; }
    public string DamageInformation { get; set; }
    public DateTime Date { get; set; }
    public bool Approved { get; set; }

    public DamageReport(int id, int clientId, int boatId, string damageInformation, DateTime date, bool approved)
    {
        Id = id;
        ClientId = clientId;
        BoatId = boatId;
        DamageInformation = damageInformation;
        Date = date;
        Approved = approved;
    }
}

