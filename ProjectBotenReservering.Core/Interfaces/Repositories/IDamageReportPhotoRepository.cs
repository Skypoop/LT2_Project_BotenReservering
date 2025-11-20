using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IDamageReportPhotoRepository
{
    public DamageReportPhoto Add(DamageReportPhoto item);
    public DamageReportPhoto? Get(int id);
    public List<DamageReportPhoto> GetByDamageReportId(int damageReportId);
    public void Delete(int id);
}

