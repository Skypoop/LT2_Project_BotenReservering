namespace ProjectBotenReservering.Core.Models;

public class DamageReportPhoto
{
    public int Id { get; set; }
    public int DamageReportId { get; set; }
    public string Url { get; set; }

    public DamageReportPhoto(int id, int damageReportId, string url)
    {
        Id = id;
        DamageReportId = damageReportId;
        Url = url;
    }
}

